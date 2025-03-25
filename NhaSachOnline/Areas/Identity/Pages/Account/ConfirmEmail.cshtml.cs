using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace NhaSachOnline.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ConfirmEmailModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                StatusMessage = "Lỗi: Thiếu thông tin xác nhận email.";
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                StatusMessage = $"Lỗi: Không tìm thấy người dùng với ID '{userId}'.";
                return Page();
            }

            try
            {
                // Giải mã code
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi giải mã mã xác nhận: {ex.Message}";
                return Page();
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                StatusMessage = "Cảm ơn bạn đã xác nhận email. Bạn có thể đăng nhập ngay bây giờ.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            else
            {
                // Hiển thị chi tiết lỗi
                StatusMessage = "Lỗi khi xác nhận email: " + string.Join(", ", result.Errors.Select(e => e.Description));
                return Page();
            }
        }
    }
}