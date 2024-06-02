using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyKho.ViewModels.PhieuNhapViewModels;

namespace QuanLyKho.ViewModels.PhieuXuatViewModels
{
    public class PhieuXuatDetailsViewModel
    {
        public string MaPhieuXuat { get; set; }
        public string MaNguoiDung { get; set; }
        public string HoVaTen { get; set; }
        public string NguoiYeuCau { get; set; }
        public string NgayXuat { get; set; }
        public string TrangThai { get; set; }
        public List<SelectListItem> TrangThaiList { get; set; }
        public string SelectedTrangThai { get; set; }
        public string SelectedTrangThaiDisplayName { get; set; }
        public decimal TongTien { get; set; }
        public string TongTienFormat => TongTien.ToString("N0");
        public string GhiChu { get; set; }
        public List<ThongTinThietBiDetailsItem> ThongTinThietBis { get; set; }
    }
}
