using Learnova.Domain.Entities;
using Learnova.Infrastructure.Data.Context;
using Learnova.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Infrastructure.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly LearnoveDbContext _learnoveDbContext;

        public OrderRepository(LearnoveDbContext learnoveDbContext)
        {
            _learnoveDbContext = learnoveDbContext;
        }

        public async Task<Order> AddAsync(Order order)
        {
            _learnoveDbContext.Orders.Add(order);
            await _learnoveDbContext.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _learnoveDbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
        {
            return await _learnoveDbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Payment)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _learnoveDbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Payment)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order> UpdateAsync(Order order)
        {
            _learnoveDbContext.Orders.Update(order);
            await _learnoveDbContext.SaveChangesAsync();
            return order;
        }

        public async Task<bool> DeleteAsync(int orderId)
        {
            var order = await _learnoveDbContext.Orders.FindAsync(orderId);
            if (order == null)
                return false;

            _learnoveDbContext.Orders.Remove(order);
            await _learnoveDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Order>> GetByPaymentStatusAsync(string paymentStatus)
        {
            return await _learnoveDbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Payment)
                .Where(o => o.PaymentStatus == paymentStatus)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetByPaymentIdAsync(int paymentId)
        {
            return await _learnoveDbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.PaymentId == paymentId);
        }
    }
}