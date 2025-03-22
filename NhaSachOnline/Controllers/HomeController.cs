using Microsoft.AspNetCore.Mvc;
using NhaSachOnline.Models;

namespace NhaSachOnline.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public HomeController(IProductRepository productRepo, IOrderRepository orderRepo)
        {
            _productRepository = productRepo;
            _orderRepository = orderRepo;
        }

        public IActionResult Index()
        {
            // Lấy 3 sản phẩm mới nhất
            var topProducts = _productRepository.GetAllProduct()
                .OrderByDescending(p => p.Id)
                .Take(3)
                .ToList(); // ToList() để đảm bảo truy vấn được thực thi

            // Lấy 3 đơn hàng mới nhất
            var recentOrders = _orderRepository.Orders
                .OrderByDescending(o => o.OrderPlaced)
                .Take(3)
                .ToList(); // ToList() để đảm bảo truy vấn được thực thi

            ViewBag.TopProducts = topProducts;
            ViewBag.RecentOrders = recentOrders;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}