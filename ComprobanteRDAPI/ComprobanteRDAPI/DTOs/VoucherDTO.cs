namespace ComprobanteRDAPI.DTOs
{
    public class VoucherDTO
    {
        public int Id { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; }
        public string? BankReferenceNumber { get; set; }
        public string Status { get; set; } = "Pendiente";
        public string? ImageURL { get; set; } = string.Empty;
        public CustomerDTO Customer { get; set; } = new();
    }
}
