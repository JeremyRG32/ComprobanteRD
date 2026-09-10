using System.ComponentModel.DataAnnotations;

namespace ComprobanteRDAPI.DTOs
{
    public class RejectVoucherDTO
    {
        [Required(ErrorMessage = "Debe indicar el motivo del rechazo.")]
        public string Reason { get; set; } = string.Empty;
    }
}
