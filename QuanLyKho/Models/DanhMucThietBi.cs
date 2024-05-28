using System;
using System.Collections.Generic;

namespace QuanLyKho.Models;

public partial class DanhMucThietBi
{
    public string MaDanhMuc { get; set; } = null!;

    public string TenDanhMuc { get; set; } = null!;

    public string? MoTa { get; set; }

    public virtual ICollection<ThongTinThietBi> ThongTinThietBis { get; set; } = new List<ThongTinThietBi>();
}
