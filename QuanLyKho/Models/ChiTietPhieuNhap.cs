using System;
using System.Collections.Generic;

namespace QuanLyKho.Models;

public partial class ChiTietPhieuNhap
{
    public string? MaPhieuNhap { get; set; }

    public string? MaThietBi { get; set; }

    public int SoLuongNhap { get; set; }

    public decimal DonGia { get; set; }

    public double ThanhTien { get; set; }

    public virtual PhieuNhapHang? MaPhieuNhapNavigation { get; set; }

    public virtual ThongTinThietBi? MaThietBiNavigation { get; set; }
}
