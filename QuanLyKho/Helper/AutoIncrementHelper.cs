using QuanLyKho.Models;

namespace QuanLyKho.Helper
{
    public class AutoIncrementHelper
    {
        public static string TaoMaNguoiDungMoi(QuanlykhoContext context)
        {
            var maNguoiDungLonNhat = context.TaiKhoans.OrderByDescending(tk => tk.MaNguoiDung).FirstOrDefault()?.MaNguoiDung;

            if (maNguoiDungLonNhat == null)
            {
                return "ND01";
            }

            int soHienTai = int.Parse(maNguoiDungLonNhat.Substring(2));
            int soMoi = soHienTai + 1;
            return "ND" + soMoi.ToString("D2");
        }

        public static string TaoMaNhaCungCapMoi(QuanlykhoContext context)
        {
            var maNhaCungCapLonNhat = context.NhaCungCaps.OrderByDescending(tk => tk.MaNhaCungCap).FirstOrDefault()?.MaNhaCungCap;

            if (maNhaCungCapLonNhat == null)
            {
                return "NC01";
            }

            int soHienTai = int.Parse(maNhaCungCapLonNhat.Substring(2));
            int soMoi = soHienTai + 1;
            return "NC" + soMoi.ToString("D2");
        }

        public static string TaoMaThietBiMoi(QuanlykhoContext context)
        {
            var maThietBiLonNhat = context.ThongTinThietBis.OrderByDescending(tk => tk.MaThietBi).FirstOrDefault()?.MaThietBi;

            if (maThietBiLonNhat == null)
            {
                return "TB01";
            }

            int soHienTai = int.Parse(maThietBiLonNhat.Substring(2));
            int soMoi = soHienTai + 1;
            return "TB" + soMoi.ToString("D2");
        }

        public static string TaoMaDanhMucMoi(QuanlykhoContext context)
        {
            var maDanhMucLonNhat = context.DanhMucThietBis.OrderByDescending(tk => tk.MaDanhMuc).FirstOrDefault()?.MaDanhMuc;

            if (maDanhMucLonNhat == null)
            {
                return "DM01";
            }

            int soHienTai = int.Parse(maDanhMucLonNhat.Substring(2));
            int soMoi = soHienTai + 1;
            return "DM" + soMoi.ToString("D2");
        }

        public static string TaoMaPhieuNhapMoi(QuanlykhoContext context)
        {
            var maPhieuNhapLonNhat = context.PhieuNhapHangs.OrderByDescending(tk => tk.MaPhieuNhap).FirstOrDefault()?.MaPhieuNhap;

            if (maPhieuNhapLonNhat == null)
            {
                return "PN01";
            }

            int soHienTai = int.Parse(maPhieuNhapLonNhat.Substring(2));
            int soMoi = soHienTai + 1;
            return "PN" + soMoi.ToString("D2");
        }

        public static string TaoMaPhieuXuatMoi(QuanlykhoContext context)
        {
            var maPhieuXuatLonNhat = context.PhieuXuatHangs.OrderByDescending(tk => tk.MaPhieuXuat).FirstOrDefault()?.MaPhieuXuat;

            if (maPhieuXuatLonNhat == null)
            {
                return "PX01";
            }

            int soHienTai = int.Parse(maPhieuXuatLonNhat.Substring(2));
            int soMoi = soHienTai + 1;
            return "PX" + soMoi.ToString("D2");
        }
    }
}
