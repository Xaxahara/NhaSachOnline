using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NhaSachOnline.Models;

namespace NhaSachOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository repo)
        {
            _productRepository = repo;
        }

        public IActionResult Index()
        {
            try
            {
                var products = _productRepository.GetAllProduct().ToList();
                return View(products);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải danh sách sản phẩm: {ex.Message}";
                return View(new List<Product>());
            }
        }

        public IActionResult Details(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                return View("NotFound");
            }
            return View(product);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_productRepository.GetAllCategories(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new DirectoryNotFoundException());
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            imageFile.CopyTo(stream);
                        }
                        product.ImagePath = $"/images/products/{fileName}";
                    }
                    _productRepository.AddProduct(product);
                    TempData["SuccessMessage"] = "Sản phẩm đã được thêm thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi thêm sản phẩm: {ex.Message}");
                }
            }
            ViewBag.Categories = new SelectList(_productRepository.GetAllCategories(), "Id", "Name");
            return View(product);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                return View("NotFound");
            }
            ViewBag.Categories = new SelectList(_productRepository.GetAllCategories(), "Id", "Name");
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new DirectoryNotFoundException());
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            imageFile.CopyTo(stream);
                        }
                        product.ImagePath = $"/images/products/{fileName}";
                    }
                    _productRepository.UpdateProduct(product);
                    TempData["SuccessMessage"] = "Sản phẩm đã được cập nhật thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi cập nhật sản phẩm: {ex.Message}");
                }
            }
            ViewBag.Categories = new SelectList(_productRepository.GetAllCategories(), "Id", "Name");
            return View(product);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                return View("NotFound");
            }
            return View(product);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _productRepository.DeleteProduct(id);
                TempData["SuccessMessage"] = "Sản phẩm đã được xóa thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi xóa sản phẩm: {ex.Message}");
                return View("Delete", _productRepository.GetProductById(id));
            }
        }
    }
}