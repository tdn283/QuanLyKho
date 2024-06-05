using QuanLyKho.ViewModels.OtherViewModels;

namespace QuanLyKho.ViewModels.NhaCungCapViewModel
{
    public class NhaCungCapIndexViewModel
    {
        public IEnumerable<NhaCungCapViewModel> NhaCungCaps { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}
