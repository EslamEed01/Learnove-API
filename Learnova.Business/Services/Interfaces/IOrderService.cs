using Learnova.Business.Abstraction;
using Learnova.Business.DTOs.Contract.Orders;

namespace Learnova.Business.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Result<OrderResponse>> CreateOrderAsync(string userId, CreateOrderRequest request, CancellationToken cancellationToken = default);
        Task<Result<OrderResponse>> GetOrderAsync(int orderId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<OrderResponse>>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<OrderResponse>>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
        Task<Result> UpdateOrderStatusAsync(int orderId, string paymentStatus, CancellationToken cancellationToken = default);
        Task<Result> CancelOrderAsync(int orderId, CancellationToken cancellationToken = default);
        Task<int?> GetOrderIdByPaymentIntentAsync(string paymentIntentId, CancellationToken cancellationToken = default);
    }
}