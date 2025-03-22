using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NhaSachOnline.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace NhaSachOnline.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly Cart _cart;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public OrderController(IOrderRepository repo, Cart cartService, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _orderRepository = repo;
            _cart = cartService;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [Authorize]
        public IActionResult Checkout()
        {
            if (!_cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index", "Cart");
            }
            ViewBag.Cart = _cart; // Truyền Cart vào ViewBag
            return View(new Order());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            if (!_cart.Items.Any())
            {
                ModelState.AddModelError("", "Giỏ hàng của bạn đang trống.");
                ViewBag.Cart = _cart;
                return View(order);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    order.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    order.Total = _cart.ComputeTotalValue();
                    _orderRepository.SaveOrder(order);

                    var user = await _userManager.GetUserAsync(User);
                    if (user != null)
                    {
                        var emailBody = $"<h3>Xin chào {order.Name},</h3>" +
                                        $"<p>Cảm ơn bạn đã đặt hàng tại Nhà Sách Online!</p>" +
                                        $"<p>Mã đơn hàng của bạn: <strong>#{order.OrderId}</strong></p>" +
                                        $"<p>Tổng tiền: <strong>{order.Total.ToString("N0", new System.Globalization.CultureInfo("vi-VN"))} VND</strong></p>" +
                                        $"<p>Chúng tôi sẽ xử lý đơn hàng của bạn sớm nhất có thể.</p>" +
                                        $"<p>Trân trọng,<br/>Nhà Sách Online</p>";
                        await _emailSender.SendEmailAsync(user.Email, "Xác nhận đơn hàng", emailBody);
                    }

                    _cart.Clear();
                    return RedirectToAction("Completed");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi đặt hàng: {ex.Message}");
                    ViewBag.Cart = _cart;
                }
            }
            return View(order);
        }

        public IActionResult Completed()
        {
            return View();
        }
    }
}