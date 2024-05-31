namespace QuanLyKho.ViewModels.PhieuNhapViewModels
{
    public class PhieuNhapViewModel
    {
        public string MaPhieuNhap { get; set; }
        public string MaNguoiDung { get; set; }
        public string HoVaTen { get; set; }
        public string NgayNhap { get; set; }
        public string TrangThai { get; set; }
        public decimal TongTien { get; set; }
        public string TongTienFormat => TongTien.ToString("N0");
        public string? GhiChu { get; set; }
    }
}
