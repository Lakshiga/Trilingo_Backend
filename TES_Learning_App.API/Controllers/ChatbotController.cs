using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Chatbot;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Only authenticated admins can use chatbot
    public class ChatbotController : BaseApiController
    {
        private readonly IChatbotService _chatbotService;

        public ChatbotController(IChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatbotRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return Ok(new ChatbotResponse
                {
                    IsSuccess = false,
                    Error = "Message is required"
                });
            }

            try
            {
                var response = await _chatbotService.GetResponseAsync(request);
                
                // Always return 200 OK, but include IsSuccess flag in response
                // This allows frontend to handle errors gracefully
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new ChatbotResponse
                {
                    IsSuccess = false,
                    Error = $"An error occurred: {ex.Message}"
                });
            }
        }

        [HttpPost("reload-knowledge")]
        [Authorize(Roles = "Admin")] // Only admins can reload knowledge base
        public async Task<IActionResult> ReloadKnowledgeBase()
        {
            var knowledgeBase = await _chatbotService.LoadKnowledgeBaseAsync();
            return Ok(new { message = "Knowledge base reloaded", content = knowledgeBase });
        }
    }
}

