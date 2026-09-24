using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ComprobanteRDAPI.Services
{
    public class WhatsAppSenderService(HttpClient http, IConfiguration config)
    {
        private string Url =>
       $"https://graph.facebook.com/v26.0/{config["WhatsAppSettings:PhoneNumberId"]}/messages";

        public Task SendTextAsync(string to, string text) =>
            PostAsync(new { messaging_product = "whatsapp", to, type = "text", text = new { body = text } });

        public Task SendMenuAsync(string to) =>
            PostAsync(new
            {
                messaging_product = "whatsapp",
                to,
                type = "interactive",
                interactive = new
                {
                    type = "button",
                    body = new { text = "Hola 👋 ¿Qué deseas hacer?" },
                    action = new
                    {
                        buttons = new[]
                        {
                        new { type = "reply", reply = new { id = "send_voucher", title = "Enviar comprobante" } },
                        new { type = "reply", reply = new { id = "check_status", title = "Consultar estado" } },
                        new { type = "reply", reply = new { id = "talk_agent",   title = "Hablar con alguien" } }
                        }
                    }
                }
            });

        private async Task PostAsync(object body)
        {
            var res = await http.PostAsJsonAsync(Url, body);
            res.EnsureSuccessStatusCode();
        }

        public async Task SendDocumentAsync(string to, Stream pdfStream, string fileName)
        {
            var accessToken = config["WhatsAppSettings:AccessToken"];
            var phoneNumberId = config["WhatsAppSettings:PhoneNumberId"];
            var baseUrl = $"https://graph.facebook.com/v26.0/{phoneNumberId}";

            // Upload the PDF and get a media id
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent("whatsapp"), "messaging_product");
            form.Add(new StringContent("application/pdf"), "type");

            var fileContent = new StreamContent(pdfStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            form.Add(fileContent, "file", fileName);

            // Create the request and add the Bearer in the header for Auth
            var uploadRequest = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/media") { Content = form };
            uploadRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Send the request and read as JSON
            var uploadResponse = await http.SendAsync(uploadRequest);
            var uploadJson = await uploadResponse.Content.ReadAsStringAsync();
            if (!uploadResponse.IsSuccessStatusCode)
                throw new Exception($"Error al subir el PDF a Meta: {uploadResponse.StatusCode} {uploadJson}");

            using var doc = JsonDocument.Parse(uploadJson);
            var mediaId = doc.RootElement.GetProperty("id").GetString();

            // Send the document message
            var payload = new
            {
                messaging_product = "whatsapp",
                to,
                type = "document",
                document = new { id = mediaId, filename = fileName }
            };

            var messageRequest = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/messages")
            {
                Content = JsonContent.Create(payload, options: new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                })
            };
            messageRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var messageResponse = await http.SendAsync(messageRequest);
            if (!messageResponse.IsSuccessStatusCode)
            {
                var error = await messageResponse.Content.ReadAsStringAsync();
                throw new Exception($"Error al enviar el documento: {messageResponse.StatusCode} {error}");
            }
        }
    }
}
