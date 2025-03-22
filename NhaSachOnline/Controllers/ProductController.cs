using Microsoft.AspNetCore.Mvc;
using NhaSachOnline.Models;
using NhaSachOnline.Models.ViewModels;

namespace NhaSachOnline.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly int PageSize = 6;

        public ProductController(IProductRepository repo)
        {
            _productRepository = repo;
        }

        public IActionResult List(int categoryId = 0, int pageNumber = 1, string searchQuery = null)
        {
            var productsQuery = _productRepository.GetAllProduct();

            if (categoryId != 0)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                productsQuery = productsQuery.Where(p =>
                    p.Name.ToLower().Contains(searchQuery) ||
                    (p.Author != null && p.Author.ToLower().Contains(searchQuery)) ||
                    (p.Category != null && p.Category.Name.ToLower().Contains(searchQuery)));
            }

            var totalProducts = productsQuery.Count();
            var products = productsQuery
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var viewModel = new ProductListViewModel
            {
                Products = products,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = PageSize,
                    TotalItems = totalProducts
                },
                Categories = _productRepository.GetAllCategories().ToList(),
                CurrentCategoryId = categoryId,
                SearchQuery = searchQuery
            };

            return View(viewModel);
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
    }
}