using System;
using System.Collections.Generic;

namespace QuanLyKho.Models;

public partial class PhieuXuatHang
{
    public string MaPhieuXuat { get; set; } = null!;

    public string? MaNguoiDung { get; set; }

    public DateTime NgayXuat { get; set; }

    public string TrangThai { get; set; } = null!;
    public decimal TongTien { get; set; }

    public string? GhiChu { get; set; }

    public string? NhanVienXuat { get; set; }

    public virtual TaiKhoan? MaNguoiDungNavigation { get; set; }
}
