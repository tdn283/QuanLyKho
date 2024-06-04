using System.ComponentModel.DataAnnotations;

namespace QuanLyKho.ViewModels.AccountViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string TenDangNhap { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
        public string MatKhau { get; set; }

        public bool RememberMe { get; set; }
    }
}
