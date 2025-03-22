using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NhaSachOnline.Data;
using NhaSachOnline.Models;

namespace NhaSachOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ApplicationDbContext _context;

        public OrderController(IOrderRepository repo, ApplicationDbContext context)
        {
            _orderRepository = repo;
            _context = context;
        }

        public ViewResult Index()
        {
            var orders = _orderRepository.Orders.ToList();
            return View(orders);
        }

        public ViewResult Details(int id)
        {
            var order = _orderRepository.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return View("NotFound");
            }
            return View(order);
        }

        [HttpGet]
        public IActionResult UpdateStatus(int id)
        {
            var order = _orderRepository.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return View("NotFound");
            }

            ViewBag.Statuses = new SelectList(new[]
            {
                "Chờ xử lý",
                "Đang giao",
                "Hoàn thành"
            });

            return View(order);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return View("NotFound");
            }

            order.Status = status;
            if (status == "Hoàn thành")
            {
                order.Shipped = true;
            }
            else
            {
                order.Shipped = false;
            }

            _context.Orders.Update(order);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công!";
            return RedirectToAction("Index");
        }
    }
}