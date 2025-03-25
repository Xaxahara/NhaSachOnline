using Microsoft.AspNetCore.Mvc;
using NhaSachOnline.Models;

namespace NhaSachOnline.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly Cart _cart;

        public CartController(IProductRepository productRepo, Cart cartService)
        {
            _productRepository = productRepo;
            _cart = cartService;
        }

        public IActionResult Index()
        {
            return View(_cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = _productRepository.GetProductById(productId);
            if (product == null)
            {
                return NotFound();
            }

            try
            {
                _cart.AddItem(product, quantity);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            _cart.RemoveItem(productId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            var product = _productRepository.GetProductById(productId);
            if (product == null)
            {
                return NotFound();
            }

            try
            {
                _cart.UpdateQuantity(productId, quantity);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Checkout()
        {
            if (!_cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction(nameof(Index));
            }
            return View(_cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(Order order)
        {
            if (!_cart.Items.Any())
            {
                ModelState.AddModelError("", "Giỏ hàng của bạn đang trống!");
                return View(_cart);
            }

            if (ModelState.IsValid)
            {
                order.DetailItems = _cart.Items.Select(item => new OrderDetail
                {
                    ProductId = item.Product.Id,
                    Product = item.Product,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                }).ToList();

                order.Total = _cart.ComputeTotalValue();
                order.OrderPlaced = DateTime.Now;
                order.Status = "Chờ xử lý";

                IOrderRepository orderRepo = HttpContext.RequestServices.GetService<IOrderRepository>();
                orderRepo.SaveOrder(order);

                _cart.Clear();
                return RedirectToAction("OrderCompleted", new { orderId = order.OrderId });
            }

            return View(_cart);
        }

        public IActionResult OrderCompleted(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}