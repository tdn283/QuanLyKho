using QuanLyKho.Models;

namespace QuanLyKho.Data.Helper
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
    }
}
