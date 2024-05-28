using System;
using System.Collections.Generic;

namespace QuanLyKho.Models;

public partial class TaiKhoan
{
    public string MaNguoiDung { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string MaVaiTro { get; set; } = null!;

    public string HoVaTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string SoDienThoai { get; set; } = null!;

    public string? TrangThai { get; set; }

    public virtual VaiTro MaVaiTroNavigation { get; set; } = null!;

    public virtual ICollection<PhieuNhapHang> PhieuNhapHangs { get; set; } = new List<PhieuNhapHang>();

    public virtual ICollection<PhieuXuatHang> PhieuXuatHangs { get; set; } = new List<PhieuXuatHang>();
}
