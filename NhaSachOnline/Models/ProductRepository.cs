using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NhaSachOnline.Data;
using System.Linq;

namespace NhaSachOnline.Models
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private const string ProductsCacheKey = "ProductsCache";
        private const string CategoriesCacheKey = "CategoriesCache";

        public ProductRepository(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public IQueryable<Product> GetAllProduct()
        {
            if (!_cache.TryGetValue(ProductsCacheKey, out IQueryable<Product> products))
            {
                products = _context.Products.Include(p => p.Category).AsQueryable();
                _cache.Set(ProductsCacheKey, products, TimeSpan.FromMinutes(30)); // Lưu vào cache 30 phút
            }
            return products;
        }

        public Product GetProductById(int id)
        {
            var products = GetAllProduct();
            return products.FirstOrDefault(p => p.Id == id);
        }

        public IQueryable<Category> GetAllCategories()
        {
            if (!_cache.TryGetValue(CategoriesCacheKey, out IQueryable<Category> categories))
            {
                categories = _context.Categories.AsQueryable();
                _cache.Set(CategoriesCacheKey, categories, TimeSpan.FromMinutes(30)); // Lưu vào cache 30 phút
            }
            return categories;
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            _cache.Remove(ProductsCacheKey); // Xóa cache khi có thay đổi
        }

        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
            _cache.Remove(ProductsCacheKey); // Xóa cache khi có thay đổi
        }

        public void DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
                _cache.Remove(ProductsCacheKey); // Xóa cache khi có thay đổi
            }
        }
    }
}