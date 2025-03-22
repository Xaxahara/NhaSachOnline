using System.ComponentModel.DataAnnotations;

namespace NhaSachOnline.Models
{
    public class ShippingDetails
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Vui lòng nhập số điện thoại hợp lệ")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thành phố")]
        public string City { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã bưu điện")]
        public string Zip { get; set; }
    }
}