using System;
using System.Collections.Generic;

namespace QuanLyKho.Models;

public partial class NhaCungCap
{
    public string MaNhaCungCap { get; set; } = null!;

    public string? TenNhaCungCap { get; set; }

    public string? DiaChi { get; set; }

    public string? SoDienThoai { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<ThongTinThietBi> ThongTinThietBis { get; set; } = new List<ThongTinThietBi>();
}
