using Learnova.Domain.Entities;

namespace Learnova.Infrastructure.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> AddAsync(Order order);
        Task<Order?> GetByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> UpdateAsync(Order order);
        Task<bool> DeleteAsync(int orderId);
        Task<IEnumerable<Order>> GetByPaymentStatusAsync(string paymentStatus);
        Task<Order?> GetByPaymentIdAsync(int paymentId);
    }
}