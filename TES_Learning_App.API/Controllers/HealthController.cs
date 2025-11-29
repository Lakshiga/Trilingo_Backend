using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILevelService _levelService;
        private readonly IStageService _stageService;

        public HealthController(ILevelService levelService, IStageService stageService)
        {
            _levelService = levelService;
            _stageService = stageService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> HealthCheck()
        {
            try
            {
                // Test levels service
                var levels = await _levelService.GetAllAsync();
                
                // Test stages service
                var stages = await _stageService.GetAllAsync();
                
                return Ok(new
                {
                    Status = "Healthy",
                    LevelsCount = levels.Count(),
                    StagesCount = stages.Count(),
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Unhealthy",
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }
}