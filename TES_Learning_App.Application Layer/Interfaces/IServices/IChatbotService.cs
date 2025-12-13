using TES_Learning_App.Application_Layer.DTOs.Chatbot;

namespace TES_Learning_App.Application_Layer.Interfaces.IServices
{
    public interface IChatbotService
    {
        Task<ChatbotResponse> GetResponseAsync(ChatbotRequest request);
        Task<string> LoadKnowledgeBaseAsync();
    }
}

