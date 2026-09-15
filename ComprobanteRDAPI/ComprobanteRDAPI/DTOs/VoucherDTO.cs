namespace ComprobanteRDAPI.DTOs
{
    public class VoucherDTO
    {
        public int Id { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; }
        public string? BankReferenceNumber { get; set; }
        public string Status { get; set; } = "Pending";
        public string? ImageURL { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
    }
}
