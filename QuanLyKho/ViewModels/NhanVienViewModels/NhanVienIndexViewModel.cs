using QuanLyKho.ViewModels.OtherViewModels;

namespace QuanLyKho.ViewModels.NhanVienViewModels
{
    public class NhanVienIndexViewModel
    {
        public IEnumerable<NhanVienViewModel> NhanViens { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}