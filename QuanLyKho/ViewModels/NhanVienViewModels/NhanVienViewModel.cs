using QuanLyKho.Models;

namespace QuanLyKho.ViewModels.NhanVienViewModels
{
    public class NhanVienViewModel
    {
        public string MaNguoiDung { get; set; }

        public string TenDangNhap { get; set; }

        public string MatKhau { get; set; }

        public string MaVaiTro { get; set; }
        public string TenVaiTro { get; set; }

        public string HoVaTen { get; set; }

        public string Email { get; set; }

        public string SoDienThoai { get; set; }

        public string? TrangThai { get; set; }

        public virtual VaiTro MaVaiTroNavigation { get; set; }

        public virtual ICollection<PhieuNhapHang> PhieuNhapHangs { get; set; }

        public virtual ICollection<PhieuXuatHang> PhieuXuatHangs { get; set; }
    }
}
