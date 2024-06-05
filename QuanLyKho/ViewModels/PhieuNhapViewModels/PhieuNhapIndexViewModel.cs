using QuanLyKho.ViewModels.OtherViewModels;

namespace QuanLyKho.ViewModels.PhieuNhapViewModels
{
    public class PhieuNhapIndexViewModel
    {
        public List<PhieuNhapViewModel> PhieuNhaps { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}
