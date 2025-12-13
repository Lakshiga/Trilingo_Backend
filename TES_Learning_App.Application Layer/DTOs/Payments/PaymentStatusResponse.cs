namespace TES_Learning_App.Application_Layer.DTOs.Payments
{
    public class PaymentStatusResponse
    {
        public bool IsSuccess { get; set; }
        public bool HasAccess { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
    }
}

