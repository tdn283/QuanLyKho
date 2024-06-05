using QuanLyKho.ViewModels.OtherViewModels;

namespace QuanLyKho.ViewModels.DanhMucViewModels
{
    public class DanhMucIndexViewModel
    {
        public IEnumerable<DanhMucViewModel> DanhMucs { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}
