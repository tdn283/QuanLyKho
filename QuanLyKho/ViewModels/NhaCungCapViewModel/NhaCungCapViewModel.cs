using System.ComponentModel.DataAnnotations;

namespace QuanLyKho.ViewModels.NhaCungCapViewModel
{
    public class NhaCungCapViewModel
    {
        public string? MaNhaCungCap { get; set; }
        [Display(Name = "Tên nhà cung cấp")]
        [Required(ErrorMessage = "Tên nhà cung cấp không được để trống")]
        public string? TenNhaCungCap { get; set; }
        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string? DiaChi { get; set; }
        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email không được để trống")]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string? Email { get; set; }
    }
}
