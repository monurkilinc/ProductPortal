using Microsoft.EntityFrameworkCore;
using ProductPortal.Core.Entities.Concrete;
using ProductPortal.Core.Utilities.Results;
using ProductPortal.DataAccess.Abstract;
using ProductPortal.DataAccess.Contexts;

namespace ProductPortal.DataAccess.Concrete
{
    public class EfOrderRepository : EfGenericRepository<Order>, IOrderRepository
    {
        public EfOrderRepository(ProductPortalContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerId(int customerId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }
        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstAsync(o => o.Id == id);
        }
        public override async Task<Order> UpdateAsync(Order order)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingOrder = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == order.Id);

                if (existingOrder == null)
                    throw new Exception("Sipariş bulunamadı");

                // Sipariş bilgilerini güncelle
                existingOrder.Status = order.Status;
                existingOrder.PaymentStatus = order.PaymentStatus;
                existingOrder.EstimatedDeliveryDate = order.EstimatedDeliveryDate;
                existingOrder.TotalAmount = order.TotalAmount;
                existingOrder.UpdatedDate = DateTime.Now;

                await _context.Database.ExecuteSqlRawAsync($"DELETE FROM OrderItems WHERE OrderId = {order.Id}");

                foreach (var item in order.OrderItems)
                {
                    await _context.OrderItems.AddAsync(new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return existingOrder;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task DeleteAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}
