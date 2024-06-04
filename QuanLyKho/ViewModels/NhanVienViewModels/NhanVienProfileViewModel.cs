using QuanLyKho.Models;
using System.ComponentModel.DataAnnotations;

namespace QuanLyKho.ViewModels.NhanVienViewModels
{
    public class NhanVienProfileViewModel
    {
        public string MaNguoiDung { get; set; }

        public string TenDangNhap { get; set; }

        public string MaVaiTro { get; set; }
        public string TenVaiTro { get; set; }

        public string HoVaTen { get; set; }

        public string Email { get; set; }
        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        public string SoDienThoai { get; set; }
    }
}
