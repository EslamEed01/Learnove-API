using Learnova.Business.DTOs.PaymentDTO;
using Learnova.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Stripe;
using System.Security.Claims;

namespace Learnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IStripePayment _stripePayment;
        private readonly ILogger<PaymentsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IOrderService _orderService;

        public PaymentsController(
            IStripePayment stripePayment,
            ILogger<PaymentsController> logger,
            IConfiguration configuration,
            IOrderService orderService)
        {
            _stripePayment = stripePayment;
            _logger = logger;
            _configuration = configuration;
            _orderService = orderService;
        }

        /// <summary>
        /// Creates a payment intent for processing payment
        /// </summary>
        /// <param name="request">Payment intent creation request</param>
        /// <returns>Payment intent response with client secret</returns>
        [HttpPost("create-payment-intent")]
        [EnableRateLimiting("CheckoutPolicy")]
        public async Task<ActionResult<PaymentIntentResponse>> CreatePaymentIntent(
            [FromBody] CreatePaymentIntentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User not authenticated");

                var response = await _stripePayment.CreatePaymentIntentForOrderAsync(request);

                _logger.LogInformation("Payment intent created for user {UserId}, amount: {Amount}",
                    userId, request.Amount);

                return Ok(response);
            }
            catch (StripeException stripeEx)
            {
                _logger.LogError(stripeEx, "Stripe error creating payment intent");
                return BadRequest(new { error = $"Payment processing error: {stripeEx.Message}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment intent");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Handles Stripe webhooks for payment status updates
        /// </summary>
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var webhookSecret = _configuration["Stripe:WebhookSecret"];

            if (string.IsNullOrEmpty(webhookSecret))
            {
                _logger.LogError("Stripe webhook secret not configured");
                return BadRequest("Webhook not properly configured");
            }

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], webhookSecret);

                _logger.LogInformation("Received Stripe webhook: {EventType}", stripeEvent.Type);

                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        var succeededPaymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                        await HandlePaymentSucceeded(succeededPaymentIntent);
                        break;

                    case "payment_intent.canceled":
                        var canceledPaymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                        await HandlePaymentCanceled(canceledPaymentIntent);
                        break;

                    case "payment_intent.payment_failed":
                        var failedPaymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                        await HandlePaymentFailed(failedPaymentIntent);
                        break;
                }

                return Ok();
            }
            catch (StripeException)
            {
                return BadRequest("Invalid signature");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Handles successful payment events
        /// </summary>
        private async Task HandlePaymentSucceeded(PaymentIntent paymentIntent)
        {
            try
            {
                _logger.LogInformation("Payment succeeded: {PaymentIntentId}", paymentIntent.Id);

                var orderId = await _orderService.GetOrderIdByPaymentIntentAsync(paymentIntent.Id);
                if (orderId.HasValue)
                {
                    var result = await _orderService.UpdateOrderStatusAsync(orderId.Value, "Completed");
                    if (result.IsSuccess)
                    {
                        _logger.LogInformation("Order {OrderId} payment confirmed and status updated to Completed", orderId.Value);
                    }
                    else
                    {
                        _logger.LogError("Failed to update order {OrderId} status to Completed", orderId.Value);
                    }
                }
                else
                {
                    _logger.LogWarning("No order found for payment intent {PaymentIntentId}", paymentIntent.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling payment success for PaymentIntent {PaymentIntentId}", paymentIntent.Id);
            }
        }

        /// <summary>
        /// Handles canceled payment events
        /// </summary>
        private async Task HandlePaymentCanceled(PaymentIntent paymentIntent)
        {
            try
            {
                _logger.LogInformation("Payment canceled: {PaymentIntentId}", paymentIntent.Id);

                var orderId = await _orderService.GetOrderIdByPaymentIntentAsync(paymentIntent.Id);
                if (orderId.HasValue)
                {
                    var result = await _orderService.CancelOrderAsync(orderId.Value);
                    if (result.IsSuccess)
                    {
                        _logger.LogInformation("Order {OrderId} canceled successfully", orderId.Value);
                    }
                    else
                    {
                        _logger.LogError("Failed to cancel order {OrderId}", orderId.Value);
                    }
                }
                else
                {
                    _logger.LogWarning("No order found for payment intent {PaymentIntentId}", paymentIntent.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling payment cancellation for PaymentIntent {PaymentIntentId}", paymentIntent.Id);
            }
        }

        /// <summary>
        /// Handles failed payment events
        /// </summary>
        private async Task HandlePaymentFailed(PaymentIntent paymentIntent)
        {
            try
            {
                _logger.LogInformation("Payment failed: {PaymentIntentId}", paymentIntent.Id);

                var orderId = await _orderService.GetOrderIdByPaymentIntentAsync(paymentIntent.Id);
                if (orderId.HasValue)
                {
                    var result = await _orderService.UpdateOrderStatusAsync(orderId.Value, "Failed");
                    if (result.IsSuccess)
                    {
                        _logger.LogInformation("Order {OrderId} status updated to Failed", orderId.Value);
                    }
                    else
                    {
                        _logger.LogError("Failed to update order {OrderId} status to Failed", orderId.Value);
                    }
                }
                else
                {
                    _logger.LogWarning("No order found for payment intent {PaymentIntentId}", paymentIntent.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling payment failure for PaymentIntent {PaymentIntentId}", paymentIntent.Id);
            }
        }
    }
}
