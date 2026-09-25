namespace ComprobanteRDAPI.DTOs
{
    public class TransactionHistoryDTO
    {
        public int VoucherId { get; set; }
        public DateTime SentAt { get; set; }
        public string BankReferenceNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;

    }
}
