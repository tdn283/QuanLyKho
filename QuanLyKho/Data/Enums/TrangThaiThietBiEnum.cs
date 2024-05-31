using System.ComponentModel.DataAnnotations;

namespace QuanLyKho.Data.Enums
{
    public enum TrangThaiThietBiEnum
    {
        [Display(Name = "Còn Hàng")]
        ConHang,

        [Display(Name = "Hết Hàng")]
        HetHang,

        [Display(Name = "Ngừng Kinh Doanh")]
        NgungKinhDoanh
    }
}
