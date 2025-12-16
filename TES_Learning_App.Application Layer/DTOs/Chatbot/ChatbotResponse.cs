namespace TES_Learning_App.Application_Layer.DTOs.Chatbot
{
    public class ChatbotResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ConversationId { get; set; }
        public string? Error { get; set; }
        public string? ImageData { get; set; } // Base64 encoded image data
        public bool HasImage { get; set; } // Indicates if response contains an image
    }
}

