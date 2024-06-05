using System.ComponentModel.DataAnnotations;

namespace QuanLyKho.ViewModels.NhanVienViewModels
{
    public class NhanVienEditViewModel
    {
        public string MaNguoiDung { get; set; }
        public string MaVaiTro { get; set; }

        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string TenDangNhap { get; set; }
        
        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        public string HoVaTen { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email không được để trống")]
        // email validation
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; }
    }
}
