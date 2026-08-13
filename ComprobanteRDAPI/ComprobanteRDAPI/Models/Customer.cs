namespace ComprobanteRDAPI.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string WhatsAppPhone { get; set; } = string.Empty;

        //Navigation Properties
        public Company? Company { get; set; }
    }
}
