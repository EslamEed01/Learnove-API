using AutoMapper;
using Learnova.Business.Abstraction;
using Learnova.Business.DTOs.Contract.Orders;
using Learnova.Business.Errors;
using Learnova.Business.Services.Interfaces;
using Learnova.Domain.Entities;
using Learnova.Infrastructure.Data.Context;
using Learnova.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Business.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly LearnoveDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            UserManager<AppUser> userManager,
            LearnoveDbContext context,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<OrderResponse>> CreateOrderAsync(string userId, CreateOrderRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure<OrderResponse>(OrderErrors.UserNotFound);

            if (request.CourseIds == null || !request.CourseIds.Any())
                return Result.Failure<OrderResponse>(OrderErrors.EmptyOrder);

            var uniqueCourseIds = request.CourseIds.Distinct().ToList();

            var courses = await _context.Courses
                .Where(c => uniqueCourseIds.Contains(c.CourseId))
                .ToListAsync(cancellationToken);

            if (courses.Count != uniqueCourseIds.Count)
                return Result.Failure<OrderResponse>(OrderErrors.InvalidCourses);

            var existingEnrollments = await _context.Enrollments
                .Where(e => e.UserId == userId && uniqueCourseIds.Contains(e.CourseId))
                .Select(e => e.CourseId)
                .ToListAsync(cancellationToken);

            if (existingEnrollments.Any())
            {
                return Result.Failure<OrderResponse>(new Error(
                    "Order.AlreadyEnrolled",
                    $"User is already enrolled in course(s): {string.Join(", ", existingEnrollments)}",
                    400));
            }

            var totalPrice = courses.Sum(c => c.Price);

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var payment = new Payment
                    {
                        PaymentDate = DateTime.UtcNow,
                        Amount = totalPrice,
                        PaymentMethod = request.PaymentMethod,
                        PaymentStatus = "Pending"
                    };

                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync(cancellationToken);

                    var order = new Order
                    {
                        UserId = userId,
                        PaymentId = payment.PaymentId,
                        OrderDate = DateTime.UtcNow,
                        TotalPrice = totalPrice,
                        PaymentMethod = request.PaymentMethod,
                        PaymentStatus = "Pending"
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync(cancellationToken);

                    var orderItems = courses.Select(course => new OrderItem
                    {
                        OrderId = order.OrderId,
                        CourseId = course.CourseId,
                        Price = course.Price
                    }).ToList();

                    _context.OrderItems.AddRange(orderItems);
                    await _context.SaveChangesAsync(cancellationToken);

                    await transaction.CommitAsync(cancellationToken);

                    var orderResponse = new OrderResponse(
                        order.OrderId,
                        order.UserId,
                        order.OrderDate,
                        order.TotalPrice,
                        order.PaymentMethod,
                        order.PaymentStatus,
                        courses.Select(c => new OrderCourseResponse(c.CourseId, c.Title, c.Price)).ToList()
                    );

                    return Result.Success(orderResponse);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }

        public async Task<Result<OrderResponse>> GetOrderAsync(int orderId, CancellationToken cancellationToken = default)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Payment)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Course)
                .FirstOrDefaultAsync(o => o.OrderId == orderId, cancellationToken);

            if (order == null)
                return Result.Failure<OrderResponse>(OrderErrors.OrderNotFound);

            var coursesResponse = order.OrderItems.Select(oi => new OrderCourseResponse(
                oi.CourseId,
                oi.Course.Title,
                oi.Price
            )).ToList();

            var orderResponse = new OrderResponse(
                order.OrderId,
                order.UserId,
                order.OrderDate,
                order.TotalPrice,
                order.PaymentMethod,
                order.PaymentStatus,
                coursesResponse
            );

            return Result.Success(orderResponse);
        }

        public async Task<Result<IEnumerable<OrderResponse>>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Course)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);

            var orderResponses = orders.Select(order => new OrderResponse(
                order.OrderId,
                order.UserId,
                order.OrderDate,
                order.TotalPrice,
                order.PaymentMethod,
                order.PaymentStatus,
                order.OrderItems.Select(oi => new OrderCourseResponse(
                    oi.CourseId,
                    oi.Course.Title,
                    oi.Price
                )).ToList()
            ));

            return Result.Success(orderResponses);
        }

        public async Task<Result<IEnumerable<OrderResponse>>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Course)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);

            var orderResponses = orders.Select(order => new OrderResponse(
                order.OrderId,
                order.UserId,
                order.OrderDate,
                order.TotalPrice,
                order.PaymentMethod,
                order.PaymentStatus,
                order.OrderItems.Select(oi => new OrderCourseResponse(
                    oi.CourseId,
                    oi.Course.Title,
                    oi.Price
                )).ToList()
            ));

            return Result.Success(orderResponses);
        }

        public async Task<Result> UpdateOrderStatusAsync(int orderId, string paymentStatus, CancellationToken cancellationToken = default)
        {
            var order = await _context.Orders
                .Include(o => o.Payment)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId, cancellationToken);

            if (order == null)
                return Result.Failure(OrderErrors.OrderNotFound);

            if (order.PaymentStatus == "Cancelled")
                return Result.Failure(OrderErrors.OrderAlreadyCancelled);

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    order.PaymentStatus = paymentStatus;

                    if (order.Payment != null)
                    {
                        order.Payment.PaymentStatus = paymentStatus;
                        if (paymentStatus == "Completed")
                        {
                            order.Payment.PaymentDate = DateTime.UtcNow;
                        }
                    }

                    if (paymentStatus == "Completed")
                    {
                        var existingEnrollments = await _context.Enrollments
                            .Where(e => e.UserId == order.UserId &&
                                   order.OrderItems.Select(oi => oi.CourseId).Contains(e.CourseId))
                            .Select(e => e.CourseId)
                            .ToListAsync(cancellationToken);

                        var newEnrollments = order.OrderItems
                            .Where(oi => !existingEnrollments.Contains(oi.CourseId))
                            .Select(oi => new Enrollment
                            {
                                UserId = order.UserId,
                                CourseId = oi.CourseId,
                                EnrollmentDate = DateTime.UtcNow,
                                Status = "Active"
                            }).ToList();

                        if (newEnrollments.Any())
                        {
                            _context.Enrollments.AddRange(newEnrollments);
                        }
                    }

                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    return Result.Success();
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }

        public async Task<Result> CancelOrderAsync(int orderId, CancellationToken cancellationToken = default)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId, cancellationToken);

            if (order == null)
                return Result.Failure(OrderErrors.OrderNotFound);

            if (order.PaymentStatus == "Completed")
                return Result.Failure(OrderErrors.OrderAlreadyPaid);

            if (order.PaymentStatus == "Cancelled")
                return Result.Failure(OrderErrors.OrderAlreadyCancelled);

            return await UpdateOrderStatusAsync(orderId, "Cancelled", cancellationToken);
        }

        public async Task<int?> GetOrderIdByPaymentIntentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(paymentIntentId))
                return null;

            var order = await _context.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Payment.PaymentIntentId == paymentIntentId, cancellationToken);

            return order?.OrderId;
        }
    }
}