using System;
using System.Collections.Generic;

namespace QuanLyKho.Models;

public partial class ThongTinThietBi
{
    public string MaThietBi { get; set; } = null!;

    public string? MaDanhMuc { get; set; }

    public string? MaNhaCungCap { get; set; }

    public string TenThietBi { get; set; } = null!;

    public string ThoiGianBaoHanh { get; set; } = null!;

    public string Model { get; set; } = null!;

    public string NhaSanXuat { get; set; } = null!;

    public string? MoTa { get; set; }

    public string TrangThai { get; set; } = null!;

    public int? SoLuongCon { get; set; }
    public decimal? GiaBan { get; set; }
    public virtual DanhMucThietBi? MaDanhMucNavigation { get; set; }

    public virtual NhaCungCap? MaNhaCungCapNavigation { get; set; }

}
