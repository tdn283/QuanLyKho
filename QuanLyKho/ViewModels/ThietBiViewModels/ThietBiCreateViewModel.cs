using System.ComponentModel.DataAnnotations;

namespace QuanLyKho.ViewModels.ThietBiViewModels
{
    public class ThietBiCreateViewModel
    {
        [Display(Name = "Tên thiết bị")]
        [Required(ErrorMessage = "Tên thiết bị không được để trống")]
        public string TenThietBi { get; set; }
        public string MaDanhMuc { get; set; }
        public string MaNhaCungCap { get; set; }
        [Display(Name = "Thời gian bảo hành")]
        [Required(ErrorMessage = "Thời gian bảo hành không được để trống")]
        public string ThoiGianBaoHanh { get; set; }
        [Display(Name = "Model")]
        [Required(ErrorMessage = "Model không được để trống")]
        public string Model { get; set; }
        [Display(Name = "Nhà sản xuất")]
        [Required(ErrorMessage = "Nhà sản xuất không được để trống")]
        public string NhaSanXuat { get; set; }
        public string? MoTa { get; set; }
        [Display(Name = "Đơn giá")]
        [Required(ErrorMessage = "Đơn giá không được để trống")]
        public double GiaBan { get; set; }
    }
}
