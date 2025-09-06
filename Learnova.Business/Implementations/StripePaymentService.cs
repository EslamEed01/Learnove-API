using Learnova.Business.DTOs.PaymentDTO;
using Learnova.Business.Services.Interfaces;
using Learnova.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Learnova.Business.Implementations
{
    public class StripePaymentService : IStripePayment
    {
        private readonly LearnoveDbContext _context;
        private readonly IConfiguration _configuration;

        public StripePaymentService(IConfiguration configuration, LearnoveDbContext context)
        {
            _configuration = configuration;
            _context = context;
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
        }

        public async Task<PaymentIntent> CreatePaymentIntentAsync(long amount, string currency)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = currency,
                PaymentMethodTypes = new List<string> { "card" },
                //AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                //{
                //    Enabled = true
                //}
            };

            var service = new PaymentIntentService();
            return await service.CreateAsync(options);
        }

        public async Task<PaymentIntentResponse> CreatePaymentIntentForOrderAsync(CreatePaymentIntentRequest request)
        {
            var amountInCents = (long)(request.Amount * 100);

            var metadata = new Dictionary<string, string>();
            if (request.OrderId.HasValue)
            {
                metadata.Add("order_id", request.OrderId.Value.ToString());
            }

            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = request.Currency,
                PaymentMethodTypes = new List<string> { "card" },
                Metadata = metadata
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            if (request.OrderId.HasValue)
            {
                await UpdatePaymentWithIntentId(request.OrderId.Value, paymentIntent.Id);
            }

            return new PaymentIntentResponse
            {
                PaymentIntentId = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret,
                Amount = request.Amount,
                Currency = request.Currency,
                Status = paymentIntent.Status
            };
        }

        private async Task UpdatePaymentWithIntentId(int orderId, string paymentIntentId)
        {
            var order = await _context.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order?.Payment != null)
            {
                order.Payment.PaymentIntentId = paymentIntentId;
                await _context.SaveChangesAsync();
            }
        }
    }
}
