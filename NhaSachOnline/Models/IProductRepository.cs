using System.Linq;

namespace NhaSachOnline.Models
{
    public interface IProductRepository
    {
        IQueryable<Product> GetAllProduct();
        Product GetProductById(int id);
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int id);
        IQueryable<Category> GetAllCategories();
    }
}