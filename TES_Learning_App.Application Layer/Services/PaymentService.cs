using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;
using TES_Learning_App.Application_Layer.DTOs.Payments;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
using TES_Learning_App.Application_Layer.Settings;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Application_Layer.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentService> _logger;
        private readonly StripeSettings _stripeSettings;

        public PaymentService(
            IUnitOfWork unitOfWork,
            ILogger<PaymentService> logger,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

            // Load Stripe settings from configuration
            _stripeSettings = new StripeSettings();
            configuration.GetSection("Stripe").Bind(_stripeSettings);

            // Initialize Stripe API key
            if (!string.IsNullOrEmpty(_stripeSettings.SecretKey))
            {
                StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
            }
        }

        public async Task<PaymentSessionResponse> CreateCheckoutSessionAsync(Guid userId, PaymentSessionRequest request)
        {
            try
            {
                // Validate level exists
                var level = await _unitOfWork.LevelRepository.GetByIdAsync(request.LevelId);
                if (level == null)
                {
                    return new PaymentSessionResponse
                    {
                        IsSuccess = false,
                        Error = "Level not found"
                    };
                }

                // Check if Level 1 (free)
                if (request.LevelId == 1)
                {
                    // Level 1 is free, grant access directly
                    var existingPurchase = await _unitOfWork.LevelPurchaseRepository
                        .FindAsync(lp => lp.UserId == userId && lp.LevelId == request.LevelId);

                    if (!existingPurchase.Any())
                    {
                        var freePurchase = new LevelPurchase
                        {
                            UserId = userId,
                            LevelId = request.LevelId,
                            StripeSessionId = "FREE_LEVEL_1",
                            PaymentStatus = "completed",
                            Amount = 0,
                            Currency = _stripeSettings.Currency,
                            PurchasedAt = DateTime.UtcNow,
                            CompletedAt = DateTime.UtcNow
                        };

                        await _unitOfWork.LevelPurchaseRepository.AddAsync(freePurchase);
                        await _unitOfWork.CompleteAsync();
                    }

                    return new PaymentSessionResponse
                    {
                        IsSuccess = true,
                        Message = "Level 1 is free. Access granted.",
                        SessionId = "FREE_LEVEL_1"
                    };
                }

                // Check if user already purchased this level
                var existingPaidPurchase = await _unitOfWork.LevelPurchaseRepository
                    .FindAsync(lp => lp.UserId == userId && lp.LevelId == request.LevelId && lp.PaymentStatus == "completed");

                if (existingPaidPurchase.Any())
                {
                    return new PaymentSessionResponse
                    {
                        IsSuccess = true,
                        Message = "You already have access to this level.",
                        SessionId = existingPaidPurchase.First().StripeSessionId
                    };
                }

                // Create Stripe checkout session
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new System.Collections.Generic.List<string> { "card" },
                    LineItems = new System.Collections.Generic.List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = _stripeSettings.Currency.ToLower(),
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = $"Level {request.LevelId} Access",
                                    Description = $"Unlock access to Level {request.LevelId}"
                                },
                                UnitAmount = (long)(_stripeSettings.LevelPrice * 100) // Convert to cents/paise
                            },
                            Quantity = 1
                        }
                    },
                    Mode = "payment",
                    SuccessUrl = request.SuccessUrl,
                    CancelUrl = request.CancelUrl,
                    Metadata = new System.Collections.Generic.Dictionary<string, string>
                    {
                        { "userId", userId.ToString() },
                        { "levelId", request.LevelId.ToString() }
                    }
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                // Save purchase record with pending status
                var purchase = new LevelPurchase
                {
                    UserId = userId,
                    LevelId = request.LevelId,
                    StripeSessionId = session.Id,
                    PaymentStatus = "pending",
                    Amount = _stripeSettings.LevelPrice,
                    Currency = _stripeSettings.Currency,
                    PurchasedAt = DateTime.UtcNow
                };

                await _unitOfWork.LevelPurchaseRepository.AddAsync(purchase);
                await _unitOfWork.CompleteAsync();

                return new PaymentSessionResponse
                {
                    IsSuccess = true,
                    SessionId = session.Id,
                    SessionUrl = session.Url
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error creating checkout session");
                return new PaymentSessionResponse
                {
                    IsSuccess = false,
                    Error = $"Stripe error: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating checkout session");
                return new PaymentSessionResponse
                {
                    IsSuccess = false,
                    Error = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<PaymentStatusResponse> CheckLevelAccessAsync(Guid userId, int levelId)
        {
            try
            {
                // Level 1 is always free
                if (levelId == 1)
                {
                    return new PaymentStatusResponse
                    {
                        IsSuccess = true,
                        HasAccess = true,
                        Message = "Level 1 is free"
                    };
                }

                // Check if user has completed purchase for this level
                var purchase = await _unitOfWork.LevelPurchaseRepository
                    .FindAsync(lp => lp.UserId == userId && lp.LevelId == levelId && lp.PaymentStatus == "completed");

                if (purchase.Any())
                {
                    return new PaymentStatusResponse
                    {
                        IsSuccess = true,
                        HasAccess = true,
                        Message = "You have access to this level"
                    };
                }

                return new PaymentStatusResponse
                {
                    IsSuccess = true,
                    HasAccess = false,
                    Message = "Payment required to access this level"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking level access");
                return new PaymentStatusResponse
                {
                    IsSuccess = false,
                    Error = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<PaymentStatusResponse> VerifyPaymentAsync(string sessionId)
        {
            try
            {
                // Retrieve session from Stripe
                var service = new SessionService();
                var session = await service.GetAsync(sessionId);

                if (session == null)
                {
                    return new PaymentStatusResponse
                    {
                        IsSuccess = false,
                        Error = "Session not found"
                    };
                }

                // Get purchase record
                var purchase = await _unitOfWork.LevelPurchaseRepository
                    .FindAsync(lp => lp.StripeSessionId == sessionId);

                if (!purchase.Any())
                {
                    return new PaymentStatusResponse
                    {
                        IsSuccess = false,
                        Error = "Purchase record not found"
                    };
                }

                var purchaseRecord = purchase.First();

                // Check payment status
                if (session.PaymentStatus == "paid")
                {
                    // Update purchase status to completed
                    if (purchaseRecord.PaymentStatus != "completed")
                    {
                        purchaseRecord.PaymentStatus = "completed";
                        purchaseRecord.CompletedAt = DateTime.UtcNow;
                        await _unitOfWork.LevelPurchaseRepository.UpdateAsync(purchaseRecord);
                        await _unitOfWork.CompleteAsync();
                    }

                    return new PaymentStatusResponse
                    {
                        IsSuccess = true,
                        HasAccess = true,
                        Message = "Payment verified successfully"
                    };
                }

                return new PaymentStatusResponse
                {
                    IsSuccess = true,
                    HasAccess = false,
                    Message = $"Payment status: {session.PaymentStatus}"
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error verifying payment");
                return new PaymentStatusResponse
                {
                    IsSuccess = false,
                    Error = $"Stripe error: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying payment");
                return new PaymentStatusResponse
                {
                    IsSuccess = false,
                    Error = $"Error: {ex.Message}"
                };
            }
        }
    }
}

