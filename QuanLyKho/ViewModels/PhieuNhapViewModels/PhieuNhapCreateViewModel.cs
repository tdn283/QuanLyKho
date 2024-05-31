using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using QuanLyKho.Models;

namespace QuanLyKho.ViewModels.PhieuNhapViewModels
{
    public class PhieuNhapCreateViewModel
    {
        public string MaNguoiDung { get; set; }
        [ValidateNever]
        public string HoVaTen { get; set; }
        public decimal TongTien { get; set; }
        [ValidateNever]
        public string TongTienFormat => TongTien.ToString("N0");
        public string? GhiChu { get; set; }
        public List<ThongTinThietBiCreateItem> ThongTinThietBis { get; set; }
    }

    public class ThongTinThietBiCreateItem
    {
        public string MaThietBi { get; set; }
        [ValidateNever]
        public string TenThietBi { get; set; }
        public string MaNhaCungCap { get; set; }
        [ValidateNever]
        public string TenNhaCungCap { get; set; }
        public int? SoLuongCon { get; set; }
        public decimal? GiaBan { get; set; }
        [ValidateNever]
        public string GiaBanFormat => GiaBan?.ToString("N0");
        public int SoLuongNhap { get; set; }
    }
}
