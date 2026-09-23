namespace ComprobanteRDAPI.DTOs
{
    public class CustomerDirectoryDTO
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int TotalTransactions { get; set; }
        public decimal Amount { get; set; }
    }
}
