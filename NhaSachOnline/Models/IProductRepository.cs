using System.Linq;

namespace NhaSachOnline.Models
{
    public interface IProductRepository
    {
        IQueryable<Product> GetAllProduct();
        Product GetProductById(int id);
        IQueryable<Category> GetAllCategories();
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int id);
    }
}