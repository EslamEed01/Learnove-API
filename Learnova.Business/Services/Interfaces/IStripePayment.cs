using Learnova.Business.DTOs.PaymentDTO;
using Stripe;

namespace Learnova.Business.Services.Interfaces
{
    public interface IStripePayment
    {
        Task<PaymentIntent> CreatePaymentIntentAsync(long amount, string currency);
        Task<PaymentIntentResponse> CreatePaymentIntentForOrderAsync(CreatePaymentIntentRequest request);
    }
}
