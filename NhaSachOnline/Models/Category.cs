using System.ComponentModel.DataAnnotations;

namespace NhaSachOnline.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        public string Name { get; set; }

        public string Description { get; set; }

        public List<Product> Products { get; set; }
    }
}