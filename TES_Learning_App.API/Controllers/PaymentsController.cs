using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Payments;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] PaymentSessionRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { IsSuccess = false, Error = "Invalid user" });
                }

                var response = await _paymentService.CreateCheckoutSessionAsync(userId, request);

                if (!response.IsSuccess)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new PaymentSessionResponse
                {
                    IsSuccess = false,
                    Error = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet("check-access/{levelId}")]
        public async Task<IActionResult> CheckLevelAccess(int levelId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { IsSuccess = false, Error = "Invalid user" });
                }

                var response = await _paymentService.CheckLevelAccessAsync(userId, levelId);

                if (!response.IsSuccess)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new PaymentStatusResponse
                {
                    IsSuccess = false,
                    Error = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpPost("verify-payment")]
        public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentRequest request)
        {
            try
            {
                var response = await _paymentService.VerifyPaymentAsync(request.SessionId);

                if (!response.IsSuccess)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new PaymentStatusResponse
                {
                    IsSuccess = false,
                    Error = $"Internal server error: {ex.Message}"
                });
            }
        }
    }

    public class VerifyPaymentRequest
    {
        public string SessionId { get; set; } = string.Empty;
    }
}

