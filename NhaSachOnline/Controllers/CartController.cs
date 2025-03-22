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

        // GET: /Cart
        public IActionResult Index()
        {
            return View(_cart);
        }

        // POST: /Cart/AddToCart
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = _productRepository.GetProductById(productId);
            if (product == null)
            {
                return NotFound();
            }

            _cart.AddItem(product, quantity);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/RemoveFromCart
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            _cart.RemoveItem(productId); // Sửa từ RemoveLine thành RemoveItem
            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/UpdateCart
        [HttpPost]
        public IActionResult UpdateCart(IFormCollection form)
        {
            foreach (var key in form.Keys)
            {
                if (key.StartsWith("quantity_"))
                {
                    var productId = int.Parse(key.Replace("quantity_", ""));
                    var quantity = int.Parse(form[key]);
                    _cart.UpdateQuantity(productId, quantity); // Sửa từ UpdateItem thành UpdateQuantity
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Cart/Checkout
        public IActionResult Checkout()
        {
            if (!_cart.Items.Any()) // Sửa từ Lines thành Items
            {
                return RedirectToAction(nameof(Index));
            }
            return View(_cart);
        }

        // POST: /Cart/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(Order order)
        {
            if (!_cart.Items.Any()) // Sửa từ Lines thành Items
            {
                ModelState.AddModelError("", "Giỏ hàng của bạn trống!");
                return View(_cart);
            }

            if (ModelState.IsValid)
            {
                order.DetailItems = _cart.Items.Select(item => new OrderDetail
                {
                    ProductId = item.Product.Id,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                }).ToList(); // Sửa từ Lines thành Items và gán vào DetailItems

                order.Total = _cart.ComputeTotalValue();
                order.OrderPlaced = DateTime.Now;
                order.Status = "Chờ xử lý";

                // Lưu đơn hàng vào cơ sở dữ liệu
                IOrderRepository orderRepo = HttpContext.RequestServices.GetService<IOrderRepository>();
                orderRepo.SaveOrder(order);

                // Xóa giỏ hàng sau khi thanh toán
                _cart.Clear();
                return RedirectToAction("OrderCompleted", new { orderId = order.OrderId });
            }

            return View(_cart);
        }

        // GET: /Cart/OrderCompleted
        public IActionResult OrderCompleted(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}