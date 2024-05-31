namespace QuanLyKho.ViewModels.ThietBiViewModels
{
    public class ThietBiViewModel
    {
        public string MaThietBi { get; set; }
        public string TenThietBi { get; set; }
        public string MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }
        public string MaNhaCungCap { get; set; }
        public string TenNhaCungCap { get; set; }
        public int ThoiGianBaoHanh { get; set; }
        public string Model { get; set; }
        public string NhaSanXuat { get; set; }
        public string? MoTa { get; set; }
        public string TrangThai { get; set; }
        public int? SoLuongCon { get; set; }
        public decimal? GiaBan { get; set; }
        public string GiaBanFormat => GiaBan?.ToString("N0");

    }
}
