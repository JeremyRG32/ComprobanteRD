using ComprobanteRDAPI.Data;
using ComprobanteRDAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComprobanteRDAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : Controller
    {
        private readonly AppDbContext context;

        public CustomerController(AppDbContext context)
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
        public async Task<ActionResult<IEnumerable<CustomerDirectoryDTO>>> GetAll()
        {
            int companyId = GetCompanyId();

            var customers = await context.Customers.Where(c => c.CompanyId == companyId)
                .OrderByDescending(c => c.CustomerName)
                .Select(c => new CustomerDirectoryDTO
                {
                    Id = c.Id,
                    CustomerName = c.CustomerName,
                    PhoneNumber = c.WhatsAppPhone,
                    TotalTransactions = context.Vouchers.Count(v => v.CustomerId == c.Id),
                    Amount = context.Vouchers
                        .Where(v => v.CustomerId == c.Id)
                        .Sum(v => (decimal?)v.Amount) ?? 0m
                })
                .ToListAsync();

            return Ok(customers);
        }
    }
}
