using System;
using System.Collections.Generic;

namespace QuanLyKho.Models;

public partial class ChiTietPhieuXuat
{
    public string? MaPhieuXuat { get; set; }

    public string? MaThietBi { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public virtual PhieuXuatHang? MaPhieuXuatNavigation { get; set; }

    public virtual ThongTinThietBi? MaThietBiNavigation { get; set; }
}
