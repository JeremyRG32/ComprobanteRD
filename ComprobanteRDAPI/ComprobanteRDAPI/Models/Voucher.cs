namespace ComprobanteRDAPI.Models
{
    public class Voucher
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public string? ImageUrl { get; set; }
        public string? BankReferenceNumber { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        //Navigation Properties
        public Company? Company { get; set; }
        public Customer? Customer { get; set; }
    }
}
