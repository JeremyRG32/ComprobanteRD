using ComprobanteRDAPI.Data;
using ComprobanteRDAPI.DTOs;
using ComprobanteRDAPI.Models;
using ComprobanteRDAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComprobanteRDAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VoucherController : Controller
    {
        private readonly AppDbContext context;
        private readonly WhatsAppSenderService sender;

        public VoucherController(AppDbContext context, WhatsAppSenderService sender)
        {
            this.context = context;
            this.sender = sender;
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
        public async Task<ActionResult<IEnumerable<DashboardDTO>>> GetAll()
        {
            int companyId = GetCompanyId();

            var vouchers = await context.Vouchers
                .Where(v => v.CompanyId == companyId)
                .OrderByDescending(v => v.SentAt)
                .Select(v => new DashboardDTO
                {
                    Id = v.Id,
                    SentAt = v.SentAt,
                    Amount = v.Amount,
                    Status = v.Status,
                    CustomerName = v.Customer!.CustomerName,
                })
                .ToListAsync();

            return Ok(vouchers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VoucherDTO>> GetById(int id)
        {
            int companyId = GetCompanyId();

            var voucher = await context.Vouchers
                .Where(v => v.CompanyId == companyId && v.Id == id)
                .Select(v => new VoucherDTO
                {
                    Id = v.Id,
                    Amount = v.Amount,
                    BankReferenceNumber = v.BankReferenceNumber,
                    CustomerName = v.Customer!.CustomerName,
                    CustomerPhone = v.Customer.WhatsAppPhone,
                    ImageURL = v.ImageUrl,
                    SentAt = v.SentAt,
                    Status = v.Status,
                })
                .FirstOrDefaultAsync();

            if (voucher is null)
            {
                return NotFound();
            }

            return Ok(voucher);
        }

        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> Confirm(int id, ConfirmVoucherDTO confirmVoucherDTO)
        {
            int companyId = GetCompanyId();

            var voucher = await context.Vouchers
                .Include(v => v.Customer)
                .FirstOrDefaultAsync(v => v.Id == id && v.CompanyId == companyId);

            if (voucher is null)
            {
                return NotFound();
            }

            if (voucher.Status != "Pending")
            {
                return BadRequest(new { message = $"No se puede confirmar un comprobante en estado '{voucher.Status}'." });
            }

            if (string.IsNullOrWhiteSpace(voucher.Customer?.WhatsAppPhone))
            {
                return BadRequest(new { message = $"El cliente '{voucher.Customer?.CustomerName}' no tiene un número de WhatsApp registrado." });
            }

            voucher.Status = "Approved";
            voucher.Amount = confirmVoucherDTO.Amount;
            voucher.BankReferenceNumber = confirmVoucherDTO.BankReferenceNumber;

            var receipt = new Receipt
            {
                VoucherId = voucher.Id,
                ReceiptNumber = $"REC-{DateTime.UtcNow:yyyyMMdd}-{voucher.Id}",
                IssuedAt = DateTime.UtcNow,
                SentViaWhatsApp = true
            };

            await context.Receipts.AddAsync(receipt);
            await context.SaveChangesAsync();

            await sender.SendTextAsync(voucher.Customer.WhatsAppPhone, "Su pago ha sido confirmado exitosamente ✅");
            return Ok(new
            {
                message = "Pago confirmado exitosamente.",
                receiptNumber = receipt.ReceiptNumber
            });
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(int id, RejectVoucherDTO dto)
        {
            int companyId = GetCompanyId();

            var voucher = await context.Vouchers
                .Include(v => v.Customer)
                .FirstOrDefaultAsync(v => v.Id == id && v.CompanyId == companyId);

            if (voucher is null)
            {
                return NotFound();
            }

            if (voucher.Status != "Pending")
            {
                return BadRequest(new { message = $"No se puede rechazar un comprobante en estado '{voucher.Status}'." });
            }

            if (string.IsNullOrWhiteSpace(voucher.Customer?.WhatsAppPhone))
            {
                return BadRequest(new { message = $"El cliente '{voucher.Customer?.CustomerName}' no tiene un número de WhatsApp registrado." });
            }

            voucher.Status = "Rejected";
            await context.SaveChangesAsync();

            var message = $"Hola {voucher.Customer.CustomerName}, tu comprobante no pudo ser aprobado ❌.\n\n" +
                  $"Motivo: {dto.Reason}\n\n" +
                  $"Por favor, repite el proceso enviando una nueva captura válida.";

            await sender.SendTextAsync(voucher.Customer.WhatsAppPhone, message);

            return Ok(new
            {
                message = "Comprobante rechazado exitosamente.",
                reason = dto.Reason,
                status = voucher.Status
            });
        }
    }
}
