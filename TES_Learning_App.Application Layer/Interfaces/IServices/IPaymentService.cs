using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Payments;

namespace TES_Learning_App.Application_Layer.Interfaces.IServices
{
    public interface IPaymentService
    {
        Task<PaymentSessionResponse> CreateCheckoutSessionAsync(Guid userId, PaymentSessionRequest request);
        Task<PaymentStatusResponse> CheckLevelAccessAsync(Guid userId, int levelId);
        Task<PaymentStatusResponse> VerifyPaymentAsync(string sessionId);
    }
}

