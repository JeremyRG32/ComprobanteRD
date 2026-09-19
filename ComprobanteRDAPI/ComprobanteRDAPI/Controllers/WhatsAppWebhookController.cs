using ComprobanteRDAPI.Data;
using ComprobanteRDAPI.Models;
using ComprobanteRDAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace ComprobanteRDAPI.Controllers
{
    [ApiController]
    [Route("api/webhook/whatsapp")]
    public class WhatsAppWebhookController : Controller
    {
        private readonly AppDbContext context;
        private readonly IConfiguration configuration;
        private readonly IMemoryCache cache;
        private readonly WhatsAppSenderService sender;
        private readonly IMediaStorageService mediaStorage;

        public WhatsAppWebhookController(AppDbContext context, WhatsAppSenderService sender, IMediaStorageService mediaStorage, IConfiguration configuration, IMemoryCache cache)
        {
            this.context = context;
            this.configuration = configuration;
            this.cache = cache;
            this.sender = sender;
            this.mediaStorage = mediaStorage;
        }

        [HttpGet]
        public IActionResult VerifyWebhook(
            [FromQuery(Name = "hub.mode")] string? mode,
            [FromQuery(Name = "hub.verify_token")] string? token,
            [FromQuery(Name = "hub.challenge")] string? challenge)
        {
            if (mode == "subscribe" && token == configuration["WhatsAppSettings:VerifyToken"])
            {
                return Ok(challenge);
            }
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveWebhook([FromBody] JsonElement payload)
        {
            var value = payload.GetProperty("entry")[0].GetProperty("changes")[0].GetProperty("value");

            // Delivery/read receipts have no "messages" array
            if (!value.TryGetProperty("messages", out var messages)) return Ok();

            var m = messages[0];
            var from = m.GetProperty("from").GetString()!;
            var type = m.GetProperty("type").GetString();
            var stateKey = $"state:{from}"; //Cache key to track the status of every conversation individually
            cache.TryGetValue(stateKey, out string? state);

            switch (type)
            {
                case "interactive":
                    var choice = m.GetProperty("interactive").GetProperty("button_reply")
                                  .GetProperty("id").GetString();
                    if (choice == "send_voucher")
                    {
                        cache.Set(stateKey, "AwaitingVoucher", TimeSpan.FromMinutes(30));
                        await sender.SendTextAsync(from, "Perfecto, envíame la foto de tu comprobante.");
                    }
                    else
                    {
                        await sender.SendTextAsync(from, "Esa opción estará disponible pronto.");
                    }
                    break;

                case "image" or "document" when state == "AwaitingVoucher":
                    {
                        var mediaId = m.GetProperty(type!).GetProperty("id").GetString() ?? "sin_imagen";

                        string localImagePath;

                        try
                        {
                            localImagePath = await mediaStorage.DownloadAndSaveMediaAsync(mediaId);
                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine($"[ERROR DESCARGA IMAGEN] {ex.Message}");
                            localImagePath = "/uploads/vouchers/default-placeholder.jpg";
                        }

                        string senderName = from;
                        if (value.TryGetProperty("contacts", out var contacts) && contacts.GetArrayLength() > 0
                            && contacts[0].TryGetProperty("profile", out var profile)
                            && profile.TryGetProperty("name", out var nameProp))
                        {
                            senderName = nameProp.GetString() ?? from;
                        }

                        // TEST COMPANY ID CHANGE WHEN THERE ARE MULTIPLE COMPANIES
                        int targetCompanyId = 8;

                        // 1. Upsert Customer
                        var customer = await context.Customers
                            .FirstOrDefaultAsync(c => c.CompanyId == targetCompanyId && c.WhatsAppPhone == from);

                        if (customer == null)
                        {
                            customer = new Customer
                            {
                                CompanyId = targetCompanyId,
                                CustomerName = senderName,
                                WhatsAppPhone = from
                            };
                            context.Customers.Add(customer);
                            await context.SaveChangesAsync();
                        }

                        // 2. Insert Voucher
                        var voucher = new Voucher
                        {
                            CompanyId = targetCompanyId,
                            CustomerId = customer.Id,
                            Amount = 0.00m,
                            BankReferenceNumber = "POR VALIDAR",
                            SentAt = DateTime.UtcNow,
                            Status = "Pending",
                            ImageUrl = localImagePath
                        };

                        context.Vouchers.Add(voucher);
                        await context.SaveChangesAsync();

                        Console.WriteLine($"[SUCCESS] Voucher #{voucher.Id} created for Customer #{customer.Id}");

                        // 3. Clear state and confirm to the customer
                        cache.Remove(stateKey);
                        await sender.SendTextAsync(from, "Comprobante recibido ✅ Lo estamos verificando.");
                        break;
                    }

                default: // This will execute when an image or text is sent without choosing any option
                    await sender.SendMenuAsync(from);
                    break;
            }

            return Ok();
        }

    }
}
