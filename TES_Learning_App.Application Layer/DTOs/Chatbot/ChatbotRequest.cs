namespace TES_Learning_App.Application_Layer.DTOs.Chatbot
{
    public class ChatbotRequest
    {
        public string Message { get; set; } = string.Empty;
        public string? ConversationId { get; set; } // Optional: for maintaining conversation context
    }
}

