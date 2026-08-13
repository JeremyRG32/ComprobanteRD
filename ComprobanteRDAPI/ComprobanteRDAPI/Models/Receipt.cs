namespace ComprobanteRDAPI.Models
{
    public class Receipt
    {
        public int Id { get; set; }
        public int VoucherId { get; set; }
        public string? ReceiptNumber { get; set; }
        public string? PdfUrl { get; set; }
        public DateTime IssuedAt { get; set; } = DateTime.Now;
        public bool SentViaWhatsApp { get; set; } = false;

        //Navigation Properties
        public Voucher? Voucher { get; set; }

    }
}
