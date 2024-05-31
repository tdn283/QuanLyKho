using System;
using System.Collections.Generic;

namespace QuanLyKho.Models;

public partial class PhieuNhapHang
{
    public string MaPhieuNhap { get; set; } = null!;

    public string? MaNguoiDung { get; set; }

    public DateTime NgayNhap { get; set; }

    public string TrangThai { get; set; } = null!;
    public decimal TongTien { get; set; }

    public string? GhiChu { get; set; }

    public virtual TaiKhoan? MaNguoiDungNavigation { get; set; }
}
