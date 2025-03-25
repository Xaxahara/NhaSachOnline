using Microsoft.AspNetCore.Mvc;
using NhaSachOnline.Models;
using System.Diagnostics;

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
            var topProducts = _productRepository.GetAllProduct()
                .OrderByDescending(p => p.Id)
                .Take(3)
                .ToList();

            var recentOrders = _orderRepository.Orders
                .OrderByDescending(o => o.OrderPlaced)
                .Take(3)
                .ToList();

            ViewBag.TopProducts = topProducts;
            ViewBag.RecentOrders = recentOrders;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}