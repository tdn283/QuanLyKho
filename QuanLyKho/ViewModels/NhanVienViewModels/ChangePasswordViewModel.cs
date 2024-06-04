using System.ComponentModel.DataAnnotations;

namespace QuanLyKho.ViewModels.NhanVienViewModels
{
    public class ChangePasswordViewModel
    {
        [Display(Name = "Mật khẩu cũ")]
        [Required(ErrorMessage = "Mật khẩu cũ không được để trống")]
        public string MatKhau { get; set; }
        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [MinLength(8, ErrorMessage = "Mật khẩu mới phải có ít nhất 8 ký tự")]
        public string MatKhauMoi { get; set; }
        [Display(Name = "Xác nhận mật khẩu")]
        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [Compare("MatKhauMoi", ErrorMessage = "Xác nhận mật khẩu không trùng khớp")]
        public string XacNhanMatKhau { get; set; }
    }
}
