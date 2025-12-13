namespace TES_Learning_App.Application_Layer.DTOs.Payments
{
    public class PaymentSessionResponse
    {
        public bool IsSuccess { get; set; }
        public string? SessionId { get; set; }
        public string? SessionUrl { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
    }
}

