using Learnova.Domain.Entities;

namespace Learnova.Infrastructure.Interfaces
{
    public interface IOrderItemRepository
    {
        Task<IEnumerable<OrderItem>> AddRangeAsync(IEnumerable<OrderItem> orderItems);
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId);
        Task<bool> DeleteByOrderIdAsync(int orderId);
    }
}