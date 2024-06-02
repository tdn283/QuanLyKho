using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyKho.Data.Enums;

namespace QuanLyKho.ViewModels.PhieuNhapViewModels
{
    public class PhieuNhapDetailsViewModel
    {
        public string MaPhieuNhap { get; set; }
        public string MaNguoiDung { get; set; }
        public string HoVaTen { get; set; }
        public string NgayNhap { get; set; }
        public string TrangThai { get; set; }
        public List<SelectListItem> TrangThaiList { get; set; }
        public string SelectedTrangThai { get; set; }
        public string SelectedTrangThaiDisplayName { get; set; }
        public decimal TongTien { get; set; }
        public string TongTienFormat => TongTien.ToString("N0");
        public string GhiChu { get; set; }
        public List<ThongTinThietBiDetailsItem> ThongTinThietBis { get; set; }
    }
    public class ThongTinThietBiDetailsItem
    {
        public string MaThietBi { get; set; }
        public string TenThietBi { get; set; }
        public string MaNhaCungCap { get; set; }
        public string TenNhaCungCap { get; set; }
        public int? SoLuongCon { get; set; }
        public decimal? GiaBan { get; set; }
        public string GiaBanFormat => GiaBan?.ToString("N0");
        public int SoLuongNhap { get; set; }
    }
}
