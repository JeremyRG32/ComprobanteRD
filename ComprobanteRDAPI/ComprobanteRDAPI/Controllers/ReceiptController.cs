using ComprobanteRDAPI.Data;
using ComprobanteRDAPI.DTOs;
using ComprobanteRDAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComprobanteRDAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptController : Controller
    {
        private readonly AppDbContext context;
        private readonly WhatsAppSenderService sender;
        private readonly IWebHostEnvironment env;
        private readonly IMediaStorageService mediaStorage;

        public ReceiptController(AppDbContext context, WhatsAppSenderService sender, IWebHostEnvironment env, IMediaStorageService mediaStorage)
        {
            this.context = context;
            this.sender = sender;
            this.env = env;
            this.mediaStorage = mediaStorage;
        }
        private int GetCompanyId()
        {
            var claim = HttpContext.User.FindFirst("CompanyId")?.Value;

            if (int.TryParse(claim, out int companyId))
            {
                return companyId;
            }

            throw new UnauthorizedAccessException("The token do not have a company id.");
        }

        [HttpGet("{voucherId}")]
        public async Task<IActionResult> GetDataByVoucher(int voucherId)
        {
            var companyId = GetCompanyId();

            var receipt = await context.Receipts
            .Where(r => r.VoucherId == voucherId && r.Voucher!.CompanyId == companyId)
            .Select(r => new ReceiptPdfDTO
            {
                ReceiptNumber = r.ReceiptNumber!,
                IssuedAt = r.IssuedAt,
                PdfUrl = r.PdfUrl,
                Amount = r!.Voucher!.Amount,
                BankReference = r.Voucher.BankReferenceNumber,
                CustomerName = r.Voucher.Customer != null ? r.Voucher.Customer.CustomerName : "Cliente General",
                CustomerPhone = r.Voucher.Customer != null ? r.Voucher.Customer.WhatsAppPhone : "",
                CompanyName = r.Voucher.Company!.CommercialName,
                CompanyRnc = r.Voucher.Company.TaxId!,
                CompanyPhone = r.Voucher.Company.BusinessWhatsAppNumber,
                CompanyAddress = r.Voucher.Company.Address,
            })
            .FirstOrDefaultAsync();

            if (receipt == null) return NotFound(new { message = "Recibo no encontrado." });

            return Ok(receipt);
        }

        [HttpPost("{voucherId}/pdf")]
        [RequestSizeLimit(5_000_000)]
        public async Task<IActionResult> UploadPdf(int voucherId, IFormFile file)
        {
            var companyId = GetCompanyId();

            if (file is null || file.Length == 0)
                return BadRequest(new { message = "Archivo PDF inválido." });

            // Check the real file signature, not just the content type
            using var stream = file.OpenReadStream();
            var header = new byte[4];
            await stream.ReadExactlyAsync(header);
            if (System.Text.Encoding.ASCII.GetString(header) != "%PDF")
                return BadRequest(new { message = "El archivo no es un PDF válido." });
            stream.Position = 0;

            // Look for the receipt in the database via the voucherId
            var receipt = await context.Receipts
                .Include(r => r.Voucher).ThenInclude(v => v!.Customer)
                .FirstOrDefaultAsync(r => r.VoucherId == voucherId && r.Voucher!.CompanyId == companyId);

            if (receipt == null) return NotFound(new { message = "Recibo no encontrado." });

            // Save the pdf locally
            var fileName = $"{receipt.ReceiptNumber}-{Guid.NewGuid():N}.pdf";
            receipt.PdfUrl = await mediaStorage.SaveReceiptPdfAsync(stream, fileName);

            // Send via WhatsApp
            var phone = receipt.Voucher!.Customer?.WhatsAppPhone;
            if (!string.IsNullOrEmpty(phone))
            {
                stream.Position = 0;
                await sender.SendDocumentAsync(phone, stream, fileName);
                receipt.SentViaWhatsApp = true;
            }

            await context.SaveChangesAsync();
            return Ok(new { pdfUrl = receipt.PdfUrl });
        }

    }
}
