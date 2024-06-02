namespace QuanLyKho.ViewModels.PhieuXuatViewModels
{
    public class PhieuXuatViewModel
    {
        public string MaPhieuXuat { get; set; }
        public string MaNguoiDung { get; set; }
        public string HoVaTen { get; set; }
        public string NgayXuat { get; set; }
        public string TrangThai { get; set; }
        public decimal TongTien { get; set; }
        public string TongTienFormat => TongTien.ToString("N0");
        public string? GhiChu { get; set; }
        public string? NguoiYeuCau { get; set; }
    }
}
