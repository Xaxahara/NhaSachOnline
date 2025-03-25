using NhaSachOnline.Data;
using System.Linq;

namespace NhaSachOnline.Models
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Order> Orders => _context.Orders;

        public void SaveOrder(Order order)
        {
            if (order.OrderId == 0)
            {
                order.OrderPlaced = DateTime.Now;
                _context.Orders.Add(order);
            }
            else
            {
                var existingOrder = _context.Orders.Find(order.OrderId);
                if (existingOrder != null)
                {
                    existingOrder.Name = order.Name;
                    existingOrder.Phone = order.Phone;
                    existingOrder.City = order.City;
                    existingOrder.Zip = order.Zip;
                    existingOrder.Status = order.Status;
                    existingOrder.Shipped = order.Shipped;
                    existingOrder.Total = order.Total;
                    existingOrder.DetailItems = order.DetailItems;
                }
            }
            _context.SaveChanges();
        }
    }
}