using System.ComponentModel.DataAnnotations;

namespace QuanLyKho.ViewModels.NhanVienViewModels
{
    public class NhanVienCreateViewModel
    {
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string TenDangNhap { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
        public string MatKhau { get; set; }
        public string MaVaiTro { get; set; }
        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        public string HoVaTen { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; }
    }
}
