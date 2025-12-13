using System.ComponentModel.DataAnnotations;

namespace TES_Learning_App.Application_Layer.DTOs.Payments
{
    public class PaymentSessionRequest
    {
        [Required]
        public int LevelId { get; set; }

        [Required]
        public string SuccessUrl { get; set; } = string.Empty;

        [Required]
        public string CancelUrl { get; set; } = string.Empty;
    }
}

