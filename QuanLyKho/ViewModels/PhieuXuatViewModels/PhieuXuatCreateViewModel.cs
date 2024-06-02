using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using QuanLyKho.ViewModels.PhieuNhapViewModels;

namespace QuanLyKho.ViewModels.PhieuXuatViewModels
{
    public class PhieuXuatCreateViewModel
    {
        public string MaNguoiDung { get; set; }
        [ValidateNever]
        public string HoVaTen { get; set; }
        public string NguoiYeuCau { get; set; }
        public decimal TongTien { get; set; }
        [ValidateNever]
        public string TongTienFormat => TongTien.ToString("N0");
        public string? GhiChu { get; set; }
        public List<ThongTinThietBiCreateItem> ThongTinThietBis { get; set; }
    }
}
