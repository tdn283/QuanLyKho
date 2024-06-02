using System.ComponentModel.DataAnnotations;

namespace QuanLyKho.Data.Enums
{
    public enum TrangThaiPhieuXuat
    {
        [Display(Name = "Chờ duyệt")]
        ChoDuyet,

        [Display(Name = "Đã duyệt")]
        DaDuyet,

        [Display(Name = "Đã hủy")]
        DaHuy
    }
}
