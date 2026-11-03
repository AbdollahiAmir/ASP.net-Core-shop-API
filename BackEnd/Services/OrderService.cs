using BackEnd.Data;
using BackEnd.DTOs.Order.Admin;
using BackEnd.DTOs.Order.Customer;
using BackEnd.DTOs.Order.Seller;
using BackEnd.IServices;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BackEnd.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        private static void CloseOpenSellerFulfillments(Order order)
        {
            foreach(var fulfillment in order.SellerFulfillments)
            {
                if(fulfillment.Status is SellerFulfillmentStatus.Pending or SellerFulfillmentStatus.SubmittedToAdmin)
                {
                    fulfillment.Status = SellerFulfillmentStatus.OrderClosed;
                }
            }
        }

        private static OrderDetailsDto MapOrder(Order order)
        {
            return new OrderDetailsDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                DeliveryAddress = order.DeliveryAddress,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                DecisionReason = order.DecisionReason,
                FinalizedAt = order.FinalizedAt,
                Items = order.Items.Select(item => new OrderItemDetailsDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    LineTotal = item.Quantity * item.UnitPrice
                }).ToList()
            };
        }

        private static SellerOrderDto MapSellerOrder(OrderSellerFulfillment fulfillment, string sellerId)
        {
            var order = fulfillment.Order;

            return new SellerOrderDto
            {
                OrderId = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),

                FulfillmentStatus = fulfillment.Status.ToString(),
                SubmittedAt = fulfillment.SubmittedAt,
                ReviewedAt = fulfillment.ReviewedAt,
                DecisionReason = fulfillment.DecisionReason,

                Items = order.Items.Where(i => i.SellerId == sellerId).Select(i => new SellerOrderItemDto
                {
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    LineTotal = i.Quantity * i.UnitPrice
                }).ToList()
            };
        }

        private static AdminOrderDto MapAdminOrder(Order order)
        {
            return new AdminOrderDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                CustomerUserName = order.CustomerUserName ?? string.Empty,
                DeliveryAddress = order.DeliveryAddress,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                DecisionReason = order.DecisionReason,
                FinalizedAt = order.FinalizedAt,

                Items = order.Items.Select(item => new AdminOrderItemDto
                {
                    ProductName = item.ProductName,
                    SellerUserName = item.Seller?.UserName ?? "کاربر حذف شده",
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    LineTotal = item.Quantity * item.UnitPrice
                }).ToList(),

                Sellers = order.SellerFulfillments.Select(f => new AdminOrderSellerDto
                {
                    SellerUserName = f.Seller?.UserName ?? "کاربر حذف شده",
                    FulfillmentStatus = f.Status.ToString(),
                    SubmittedAt = f.SubmittedAt,
                    ReviewedAt = f.ReviewedAt,
                    DecisionReason = f.DecisionReason,
                    SellerId = f.SellerId ?? string.Empty
                }).ToList()
            };
        }

        // Customer

        public async Task<OrderDto> CreateFromCartAsync(string customerId, CreateOrderDto dto)
        {
            var address = dto.DeliveryAddress?.Trim();
            if (string.IsNullOrWhiteSpace(address) || address.Length < 5 || address.Length > 500)
            {
                throw new ArgumentException("طول آدرس باید بین 5 تا 500 حرف باشد");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var cart = await _context.Carts.Include(c => c.Items).ThenInclude(i => i.Product).FirstOrDefaultAsync(d => d.UserId == customerId);
            if (cart is null || cart.Items.Count == 0)
            {
                throw new InvalidOperationException("سبد خرید شما خالی است");
            }

            foreach (var item in cart.Items)
            {
                if (!item.Product.IsApproved || string.IsNullOrWhiteSpace(item.Product.SellerId))
                {
                    throw new InvalidOperationException($"محصول: '{item.Product.Name}' در دسترس نیست");
                }

                if (item.Quantity < 1)
                {
                    throw new InvalidOperationException("سبد خرید تعداد نامعتبر دریافت کرد");
                }

                if (item.Product.Price <= 0)
                {
                    throw new InvalidOperationException($"محصول: '{item.Product.Name}' مبلغ نامعبر دارد");
                }
            }

            var customer = await _context.Users.Where(u => u.Id == customerId).Select(u => new { u.Id, u.UserName }).FirstOrDefaultAsync();

            if (customer is null)
            {
                throw new InvalidOperationException("حساب مشتری یافت نشد");
            }

            var order = new Order
            {
                CustomerId = customerId,
                CustomerUserName = customer.UserName,
                DeliveryAddress = address,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };

            foreach (var item in cart.Items)
            {
                var product = item.Product;

                order.Items.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    SellerId = product.SellerId!,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });
            }

            order.TotalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice);
            var sellerIds = order.Items.Select(i => i.SellerId).Distinct().ToList();
            foreach (var sellerId in sellerIds)
            {
                order.SellerFulfillments.Add(new OrderSellerFulfillment
                {
                    SellerId = sellerId,
                    Status = SellerFulfillmentStatus.Pending
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _context.CartItems.RemoveRange(cart.Items);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new OrderDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                DeliveryAddress = order.DeliveryAddress,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                ItemCount = order.Items.Count
            };
        }

        public async Task<List<OrderDetailsDto>> GetMyOrdersAsync(string customerId)
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ThenByDescending(o => o.Id)
                .ToListAsync();

            return orders.Select(MapOrder).ToList();
        }

        public async Task<OrderDetailsDto?> GetMyOrderByIdAsync(string customerId, int orderId)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .Where(o => o.Id == orderId && o.CustomerId == customerId)
                .Include(o => o.Items)
                .FirstOrDefaultAsync();

            return order is null ? null : MapOrder(order);
        }

        public async Task<OrderDetailsDto?> CancelMyOrderAsync(string customerId, int orderId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var order = await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.SellerFulfillments)
                .FirstOrDefaultAsync(o =>
                    o.Id == orderId &&
                    o.CustomerId == customerId);
            if (order is null)
            {
                return null;
            }

            if (order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("تنها سفارش های در انتظار تایید میتوانند کنسل شوند");
            }

            order.Status = OrderStatus.CancelledByCustomer;
            order.DecisionReason = "لغو توسط مشتری";
            order.FinalizedAt = DateTime.UtcNow;

            CloseOpenSellerFulfillments(order);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return MapOrder(order);
        }

        // Seller

        public async Task<List<SellerOrderDto>> GetSellerOrdersAsync(string sellerId)
        {
            var fulfillments = await _context.OrderSellerFulfillments.AsNoTracking().Where(f => f.SellerId == sellerId).Include(f => f.Order).ThenInclude(o => o.Items).OrderByDescending(f => f.Order.CreatedAt).ToListAsync();
            return fulfillments.Select(f => MapSellerOrder(f, sellerId)).ToList();
        }

        public async Task<SellerOrderDto?> GetSellerOrderByIdAsync(string sellerId, int orderId)
        {
            var fulfillment = await _context.OrderSellerFulfillments.AsNoTracking().Where(f => f.SellerId == sellerId && f.OrderId == orderId).Include(f => f.Order).ThenInclude(o => o.Items).FirstOrDefaultAsync();

            return fulfillment is null ? null : MapSellerOrder(fulfillment, sellerId);
        }

        public async Task<SellerOrderDto?> WithdrawSellerSubmissionAsync(string sellerId, int orderId)
        {
            var fulfillment = await _context.OrderSellerFulfillments
                .Include(f => f.Order)
                    .ThenInclude(o => o.Items)
                .FirstOrDefaultAsync(f =>
                    f.OrderId == orderId &&
                    f.SellerId == sellerId);

            if (fulfillment is null)
            {
                return null;
            }

            if (fulfillment.Order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("این سفارش باز نیست");
            }

            if (fulfillment.Status != SellerFulfillmentStatus.SubmittedToAdmin && fulfillment.Status != SellerFulfillmentStatus.UnableToSupply)
            {
                throw new InvalidOperationException("وضعیت فعلی قابل پس گرفتن نیست");
            }

            fulfillment.Status = SellerFulfillmentStatus.Pending;
            fulfillment.SubmittedAt = null;
            fulfillment.ReviewedAt = null;
            fulfillment.DecisionReason = null;

            await _context.SaveChangesAsync();

            return MapSellerOrder(fulfillment, sellerId);
        }

        public async Task<SellerOrderDto?> UpdateSellerFulfillmentAsync(string sellerId, int orderId, SellerFulfillmentDecisionDto dto)
        {
            var fulfillment = await _context.OrderSellerFulfillments
                .Include(f => f.Order).ThenInclude(o => o.Items)
                .Include(f => f.Order).ThenInclude(o => o.SellerFulfillments)
                .FirstOrDefaultAsync(f => f.OrderId == orderId && f.SellerId == sellerId);

            if (fulfillment is null)
            {
                return null;
            }

            if (fulfillment.Order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("این سفارش برای اعلام تصمیم باز نیست");
            }

            if (fulfillment.Status != SellerFulfillmentStatus.Pending)
            {
                throw new InvalidOperationException("شما از قبل تصمیم خود را اعلام کردید");
            }

            var reason = dto.Reason?.Trim();

            if (dto.CanSupply == true)
            {
                fulfillment.Status = SellerFulfillmentStatus.SubmittedToAdmin;
                fulfillment.SubmittedAt = DateTime.UtcNow;
                fulfillment.DecisionReason = string.IsNullOrWhiteSpace(reason) ? null : reason;
            }
            else if (dto.CanSupply == false)
            {
                if (string.IsNullOrWhiteSpace(reason))
                {
                    throw new ArgumentException(
                        "لطفا دلیل خود را برای عدم تامین اعلام کنید");
                }

                fulfillment.Status = SellerFulfillmentStatus.UnableToSupply;
                fulfillment.SubmittedAt = DateTime.UtcNow;
                fulfillment.DecisionReason = reason;
                fulfillment.ReviewedAt = null;
            }
            else
            {
                throw new ArgumentException("پاسخ فقط میتواند ردست یا غلط باشد");
            }

            await _context.SaveChangesAsync();

            return MapSellerOrder(fulfillment, sellerId);
        }

        // Admin 

        public async Task<List<AdminOrderDto>> GetAllOrdersForAdminAsync()
        {
            var orders = await _context.Orders.AsNoTracking().Include(o => o.Items).ThenInclude(i => i.Seller).Include(o => o.SellerFulfillments).ThenInclude(f => f.Seller).OrderByDescending(o => o.CreatedAt).ThenByDescending(o => o.Id).ToListAsync();

            return orders.Select(MapAdminOrder).ToList();
        }

        public async Task<AdminOrderDto?> GetOrderForAdminByIdAsync(int orderId)
        {
            var order = await _context.Orders.AsNoTracking().Include(o => o.Items).ThenInclude(i => i.Seller).Include(o => o.SellerFulfillments).ThenInclude(f => f.Seller).FirstOrDefaultAsync(o => o.Id == orderId);

            return order is null ? null : MapAdminOrder(order);
        }

        public async Task<AdminOrderDto?> ReviewSellerFulfillmentAsync(int orderId, string sellerId, AdminSellerDecisionDto dto)
        {
            var fulfillment = await _context.OrderSellerFulfillments.Include(f => f.Order).ThenInclude(o => o.Items).ThenInclude(i => i.Seller).Include(f => f.Order).ThenInclude(o => o.SellerFulfillments).ThenInclude(f => f.Seller).FirstOrDefaultAsync(f => f.OrderId == orderId && f.SellerId == sellerId);

            if (fulfillment is null)
            {
                return null;
            }

            if (fulfillment.Order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("این سفارش برای بررسی باز نیست");
            }

            if (fulfillment.Status != SellerFulfillmentStatus.SubmittedToAdmin)
            {
                throw new InvalidOperationException("این فروشنده هنوز تصمیم خود را اعلام نکرده");
            }

            var reason = dto.Reason?.Trim();

            if (dto.Approve == true)
            {
                fulfillment.Status = SellerFulfillmentStatus.ConfirmedByAdmin;
                fulfillment.DecisionReason = string.IsNullOrWhiteSpace(reason) ? null : reason;
            }
            else if (dto.Approve == false)
            {
                if (string.IsNullOrWhiteSpace(reason))
                {
                    throw new ArgumentException(
                        "برای رد تصمیم فروشنده لطفا دلیل خود را وارد کنید");
                }

                var now = DateTime.UtcNow;

                fulfillment.Status = SellerFulfillmentStatus.RejectedByAdmin;
                fulfillment.DecisionReason = reason;
                fulfillment.ReviewedAt = now;
                fulfillment.Order.Status = OrderStatus.Rejected;
                fulfillment.Order.DecisionReason =
                    $"سفارش به علت رد شدن تصمیم یکی از فروشندگان توسط مدیر، لغو شد.: {reason}";
                fulfillment.Order.FinalizedAt = now;

                CloseOpenSellerFulfillments(fulfillment.Order);
            }
            else
            {
                throw new ArgumentException("تاییدیه تنها میتواند درست یا غلط باشد");
            }

            if (dto.Approve == true)
            {
                fulfillment.ReviewedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return MapAdminOrder(fulfillment.Order);
        }

        public async Task<AdminOrderDto?> FinalApproveOrderAsync(int orderId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            var order = await _context.Orders.Include(o => o.Items).ThenInclude(i => i.Seller).Include(o => o.SellerFulfillments).ThenInclude(f => f.Seller).FirstOrDefaultAsync(o => o.Id == orderId);

            if (order is null)
            {
                return null;
            }

            if (order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("تنها سفارشات در انتظار تایید میتوانند تایید شوند");
            }

            if (order.SellerFulfillments.Count == 0)
            {
                throw new InvalidOperationException("سفارش هیچ تصمیمی از سوی فروشندگان ندارد");
            }

            if (order.SellerFulfillments.Any(f => f.Status != SellerFulfillmentStatus.ConfirmedByAdmin))
            {
                throw new InvalidOperationException("برای تایید نهایی، به تصمیم تمام فروشندگان نیاز است.");
            }

            order.Status = OrderStatus.Approved;
            order.FinalizedAt = DateTime.UtcNow;
            order.DecisionReason = null;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return MapAdminOrder(order);
        }

        public async Task<AdminOrderDto?> RejectOrderAsync(int orderId, AdminOrderDecisionDto dto)
        {
            var reason = dto.Reason?.Trim();

            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new ArgumentException("برای رد سفارش، دلیل خود را وارد کنید.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Seller)
                .Include(o => o.SellerFulfillments)
                    .ThenInclude(f => f.Seller)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order is null)
            {
                return null;
            }

            if (order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("تنها سفارشات در انتظار تایید میتوانند رد شوند");
            }

            order.Status = OrderStatus.Rejected;
            order.DecisionReason = reason;
            order.FinalizedAt = DateTime.UtcNow;

            CloseOpenSellerFulfillments(order);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return MapAdminOrder(order);
        }

        public async Task<AdminOrderDto?> WithdrawSellerApprovalAsync(int orderId, string sellerId)
        {
            var fulfillment = await _context.OrderSellerFulfillments
                .Include(f => f.Order)
                .ThenInclude(o => o.Items)
                .ThenInclude(i => i.Seller)
                .Include(f => f.Order)
                .ThenInclude(o => o.SellerFulfillments)
                .ThenInclude(f => f.Seller)
                .FirstOrDefaultAsync(f =>
                    f.OrderId == orderId &&
                    f.SellerId == sellerId);

            if (fulfillment is null)
            {
                return null;
            }

            if (fulfillment.Order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("این سفارش باز نیست");
            }

            if (fulfillment.Status != SellerFulfillmentStatus.ConfirmedByAdmin)
            {
                throw new InvalidOperationException("فقط تصمیم های تایید شده را میتوان پس گرفت");
            }

            fulfillment.Status = SellerFulfillmentStatus.SubmittedToAdmin;
            fulfillment.ReviewedAt = null;
            fulfillment.DecisionReason = null;

            await _context.SaveChangesAsync();

            return MapAdminOrder(fulfillment.Order);
        }

        public async Task<AdminOrderDto?> CancelOrderByAdminAsync(int orderId, AdminOrderDecisionDto dto)
        {
            var reason = dto.Reason?.Trim();

            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new ArgumentException("برای لغو سفارش دلیل خود را وارد کنید.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Seller)
                .Include(o => o.SellerFulfillments)
                    .ThenInclude(f => f.Seller)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order is null)
            {
                return null;
            }

            if (order.Status is OrderStatus.Rejected or OrderStatus.CancelledByCustomer or OrderStatus.CancelledByAdmin)
            {
                throw new InvalidOperationException("این سفارش قبلا لغو شده");
            }

            order.Status = OrderStatus.CancelledByAdmin;
            order.DecisionReason = reason;
            order.FinalizedAt = DateTime.UtcNow;

            CloseOpenSellerFulfillments(order);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return MapAdminOrder(order);
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order is null)
            {
                return false;
            }

            var items = await _context.OrderItems
                .Where(i => i.OrderId == orderId)
                .ToListAsync();

            var fulfillments = await _context.OrderSellerFulfillments
                .Where(f => f.OrderId == orderId)
                .ToListAsync();

            _context.OrderItems.RemoveRange(items);
            _context.OrderSellerFulfillments.RemoveRange(fulfillments);
            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
    }
}