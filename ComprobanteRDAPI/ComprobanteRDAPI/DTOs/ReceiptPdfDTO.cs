namespace ComprobanteRDAPI.DTOs
{
    public class ReceiptPdfDTO
    {
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; }
        public string? PdfUrl { get; set; }
        public decimal Amount { get; set; }
        public string? BankReference { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyRnc { get; set; } = string.Empty;
        public string? CompanyPhone { get; set; }
        public string? CompanyAddress { get; set; }

    }
}
