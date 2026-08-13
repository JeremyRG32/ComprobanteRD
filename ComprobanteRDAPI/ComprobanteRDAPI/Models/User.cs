using Microsoft.AspNetCore.Identity;

namespace ComprobanteRDAPI.Models
{
    public class User : IdentityUser<int>
    {
        public int CompanyId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Active";

        // Navigation Property - EF Core automatically binds CompanyId!
        public Company? Company { get; set; }
    }
}
