namespace ComprobanteRDAPI.DTOs
{
    public class DashboardDTO
    {
        public int Id { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending";
        public string CustomerName { get; set; } = string.Empty;
    }
}
