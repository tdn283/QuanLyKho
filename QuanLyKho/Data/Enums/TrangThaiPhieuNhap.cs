using System.ComponentModel.DataAnnotations;

namespace QuanLyKho.Data.Enums
{
    public enum TrangThaiPhieuNhap
    {
        [Display(Name = "Chờ nhập kho")]
        ChoNhapKho,

        [Display(Name = "Đã nhập kho")]
        DaNhapKho,

        [Display(Name = "Đã hủy")]
        DaHuy
    }
}
