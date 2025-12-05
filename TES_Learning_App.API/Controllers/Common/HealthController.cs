using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.API.Controllers.Common;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.Common
{
    /// <summary>
    /// Health check controller
    /// Route: api/health
    /// </summary>
    [Route("api/health")]
    [ApiController]
    [AllowAnonymous]
    public class HealthController : BaseApiController
    {
        private readonly ILevelService _levelService;
        private readonly IStageService _stageService;

        public HealthController(ILevelService levelService, IStageService stageService)
        {
            _levelService = levelService;
            _stageService = stageService;
        }

        /// <summary>
        /// Health check endpoint
        /// GET: api/health
        /// </summary>
        [HttpGet]
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

