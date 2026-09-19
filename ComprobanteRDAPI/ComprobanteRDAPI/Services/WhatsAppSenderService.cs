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
    }
}
