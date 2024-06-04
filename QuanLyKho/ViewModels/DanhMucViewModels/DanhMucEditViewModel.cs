using System.ComponentModel.DataAnnotations;

namespace QuanLyKho.ViewModels.DanhMucViewModels
{
    public class DanhMucEditViewModel
    {
        public string MaDanhMuc { get; set; }
        [Display(Name = "Tên danh mục")]
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        public string TenDanhMuc { get; set; }
        public string? MoTa { get; set; }
    }
}
