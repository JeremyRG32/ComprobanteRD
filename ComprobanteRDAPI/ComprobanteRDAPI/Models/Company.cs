namespace ComprobanteRDAPI.Models
{
    public class Company
    {
        public int Id { get; set; }
        public int SubscriptionPlanId { get; set; }
        public string CommercialName { get; set; } = string.Empty;
        public string? TaxId { get; set; }
        public string? CommercialSector { get; set; }
        public string BusinessWhatsAppNumber { get; set; } = string.Empty;
        public string? ContactEmail { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Active";

        // Navigation Property
        public SubscriptionPlan? SubscriptionPlan { get; set; }
    }
}
