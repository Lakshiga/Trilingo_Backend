namespace TES_Learning_App.Application_Layer.Settings
{
    public class StripeSettings
    {
        public string PublishableKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string Currency { get; set; } = "INR";
        public decimal LevelPrice { get; set; } = 350;
    }
}

