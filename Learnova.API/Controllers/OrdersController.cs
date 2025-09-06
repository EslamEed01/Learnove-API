using Learnova.Business.Abstraction;
using Learnova.Business.DTOs.Contract.Orders;
using Learnova.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Learnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Create a new order with pending status
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
        {
            // Validate request model
            if (request == null)
                return BadRequest("Request cannot be null");

            if (request.CourseIds == null || !request.CourseIds.Any())
                return BadRequest("At least one course must be specified");

            if (request.CourseIds.Any(id => id <= 0))
                return BadRequest("Invalid course IDs provided");

            if (string.IsNullOrWhiteSpace(request.PaymentMethod))
                return BadRequest("Payment method is required");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            var result = await _orderService.CreateOrderAsync(userId, request, cancellationToken);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetOrder), new { id = result.Value.OrderId }, result.Value)
                : result.ToProblem();
        }

        /// <summary>
        /// Get order by ID with all course details
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id, CancellationToken cancellationToken)
        {
            var result = await _orderService.GetOrderAsync(id, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        /// <summary>
        /// Get all orders for the current user
        /// </summary>
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders(CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            var result = await _orderService.GetUserOrdersAsync(userId, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        /// <summary>
        /// Get all orders (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllOrders(CancellationToken cancellationToken)
        {
            var result = await _orderService.GetAllOrdersAsync(cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        /// <summary>
        /// Update order payment status
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request, CancellationToken cancellationToken)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, request.PaymentStatus, cancellationToken);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        /// <summary>
        /// Cancel an order (only if not completed)
        /// </summary>
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id, CancellationToken cancellationToken)
        {
            // Check if user owns the order or is an admin
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin)
            {
                var orderResult = await _orderService.GetOrderAsync(id, cancellationToken);
                if (!orderResult.IsSuccess)
                    return orderResult.ToProblem();

                if (orderResult.Value.UserId != userId)
                    return Forbid("You can only cancel your own orders");
            }

            var result = await _orderService.CancelOrderAsync(id, cancellationToken);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        /// <summary>
        /// Get order statistics (Admin only)
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrderStatistics(CancellationToken cancellationToken)
        {
            var result = await _orderService.GetAllOrdersAsync(cancellationToken);

            if (!result.IsSuccess)
                return result.ToProblem();

            var orders = result.Value.ToList();
            var statistics = new
            {
                TotalOrders = orders.Count,
                PendingOrders = orders.Count(o => o.PaymentStatus == "Pending"),
                CompletedOrders = orders.Count(o => o.PaymentStatus == "Completed"),
                CancelledOrders = orders.Count(o => o.PaymentStatus == "Cancelled"),
                TotalRevenue = orders.Where(o => o.PaymentStatus == "Completed").Sum(o => o.TotalPrice),
                AverageOrderValue = orders.Any() ? orders.Average(o => o.TotalPrice) : 0
            };

            return Ok(statistics);
        }
    }
}