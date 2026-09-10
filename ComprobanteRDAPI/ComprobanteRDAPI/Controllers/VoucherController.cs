using ComprobanteRDAPI.Data;
using ComprobanteRDAPI.DTOs;
using ComprobanteRDAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComprobanteRDAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VoucherController : Controller
    {
        private readonly AppDbContext context;

        public VoucherController(AppDbContext context)
        {
            this.context = context;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VoucherDTO>>> GetAll()
        {
            int companyId = GetCompanyId();

            var vouchers = await context.Vouchers
                .Where(v => v.CompanyId == companyId)
                .OrderByDescending(v => v.SentAt)
                .Select(v => new VoucherDTO
                {
                    Id = v.Id,
                    SentAt = v.SentAt,
                    Amount = v.Amount,
                    BankReferenceNumber = v.BankReferenceNumber,
                    ImageURL = v.ImageUrl,
                    Status = v.Status,
                    Customer = new CustomerDTO
                    {
                        Id = v.Customer != null ? v.Customer.Id : 0,
                        CustomerName = v.Customer != null ? v.Customer.CustomerName : "Unknown",
                        PhoneNumber = v.Customer != null ? v.Customer.WhatsAppPhone : string.Empty
                    }
                })
                .ToListAsync();

            return Ok(vouchers);
        }
        [HttpPost("{id}/confirm")]
        public async Task<ActionResult> Confirm(int id)
        {
            int companyId = GetCompanyId();

            var voucher = await context.Vouchers
                .Include(v => v.Customer)
                .FirstOrDefaultAsync(v => v.Id == id && v.CompanyId == companyId);

            if (voucher == null)
            {
                return NotFound(new { message = "Comprobante no encontrado." });
            }

            if (voucher.Status == "Confirmado")
            {
                return BadRequest(new { message = "Este comprobante ya fue confirmado previamente." });
            }

            voucher.Status = "Confirmado";

            var receipt = new Receipt
            {

                VoucherId = voucher.Id,
                ReceiptNumber = $"REC-{DateTime.UtcNow:yyyyMMdd}-{voucher.Id:D4}",
                IssuedAt = DateTime.UtcNow,
                SentViaWhatsApp = false // Will be true when the Whatsapp api send the message
            };

            context.Receipts.Add(receipt);
            await context.SaveChangesAsync();

            return Ok(new
            {
                message = "Comprobante confirmado exitosamente.",
                receiptNumber = receipt.ReceiptNumber,
                status = voucher.Status
            });
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectVoucherDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int companyId = GetCompanyId();

            // 1. Buscar el comprobante asegurando pertenencia al tenant
            var voucher = await context.Vouchers
                .FirstOrDefaultAsync(v => v.Id == id && v.CompanyId == companyId);

            if (voucher == null)
            {
                return NotFound(new { message = "Comprobante no encontrado o no pertenece a su empresa." });
            }

            if (voucher.Status == "Rechazado")
            {
                return BadRequest(new { message = "Este comprobante ya se encuentra rechazado." });
            }

            // 2. Actualizar estado
            voucher.Status = "Rechazado";
            await context.SaveChangesAsync();

            return Ok(new
            {
                message = "Comprobante rechazado exitosamente.",
                reason = dto.Reason,
                status = voucher.Status
            });
        }
    }
}
