
using System.Net.Http.Headers;
using System.Text.Json;

namespace ComprobanteRDAPI.Services
{
    public class MediaStorageService : IMediaStorageService
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration config;
        private readonly IWebHostEnvironment env;

        public MediaStorageService(HttpClient httpClient, IConfiguration config, IWebHostEnvironment env)
        {
            this.httpClient = httpClient;
            this.config = config;
            this.env = env;
        }
        public async Task<string> DownloadAndSaveMediaAsync(string mediaId)
        {
            var accessToken = config["WhatsAppSettings:AccessToken"];

            // Get the metadata which contains the download URL from META Graph API
            var metadataRequest = new HttpRequestMessage(HttpMethod.Get, $"https://graph.facebook.com/v26.0/{mediaId}");

            // Add the bearer auth to the request and store the response
            metadataRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var metadataResponse = await httpClient.SendAsync(metadataRequest);

            if (!metadataResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Error al consultar metadata de Meta: {metadataResponse.StatusCode}");
            }

            var jsonString = await metadataResponse.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonString);

            // Extract the download URL and the type
            var downloadURL = doc.RootElement.GetProperty("url").GetString();
            var mimeType = doc.RootElement.TryGetProperty("mime_type", out var m) ? m.GetString() : "image/jpeg";
            var extension = mimeType switch
            {
                "image/png" => ".png",
                "image/webp" => ".webp",
                "application/pdf" => ".pdf",
                _ => ".jpg"
            };

            // Download the file with the Bearer in the Header
            var downloadRequest = new HttpRequestMessage(HttpMethod.Get, downloadURL);
            downloadRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            downloadRequest.Headers.UserAgent.ParseAdd("ComprobanteRD-Backend");

            var fileResponse = await httpClient.SendAsync(downloadRequest);

            if (!fileResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Error al descargar el archivo: ${fileResponse.StatusCode}");
            }

            var fileBytes = await fileResponse.Content.ReadAsByteArrayAsync();

            // Save the file in wwwroot/uploads/vouchers
            var uploadsFolder = Path.Combine(env.WebRootPath ?? Path.Combine(Directory.
                GetCurrentDirectory(), "wwwroot"), "uploads", "vouchers");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            await File.WriteAllBytesAsync(filePath, fileBytes);


            // Return the path for the frontend
            return $"/uploads/vouchers/{fileName}";
        }
    }
}
