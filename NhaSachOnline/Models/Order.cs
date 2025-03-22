using System.ComponentModel.DataAnnotations;

namespace NhaSachOnline.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public string UserId { get; set; }

        public List<OrderDetail> DetailItems { get; set; } = new List<OrderDetail>();

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thành phố")]
        public string City { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã bưu điện")]
        public string Zip { get; set; }

        public DateTime OrderPlaced { get; set; }

        public bool Shipped { get; set; }

        public decimal Total { get; set; }

        public string Status { get; set; } = "Chờ xử lý";
    }
}