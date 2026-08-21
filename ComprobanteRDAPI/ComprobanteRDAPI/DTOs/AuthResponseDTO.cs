namespace ComprobanteRDAPI.DTOs
{
    public class AuthResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public int CompanyId { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
