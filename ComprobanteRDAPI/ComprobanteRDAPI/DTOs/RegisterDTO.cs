using System.ComponentModel.DataAnnotations;

namespace ComprobanteRDAPI.DTOs
{
    public class RegisterDTO
    {
        // Company Data (Tenant)
        [Required, StringLength(60)]
        public string CommercialName { get; set; } = string.Empty;
        [StringLength(15)]
        public string? TaxId { get; set; } // RNC / Cedula
        [Required, StringLength(18)]
        public string BusinessWhatsAppNumber { get; set; } = string.Empty;
        public int SubscriptionPlanId { get; set; } = 1; // Default basic plan

        // User Info
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;



    }
}
