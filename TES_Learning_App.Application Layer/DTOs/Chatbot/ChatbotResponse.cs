namespace TES_Learning_App.Application_Layer.DTOs.Chatbot
{
    public class ChatbotResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ConversationId { get; set; }
        public string? Error { get; set; }
    }
}

