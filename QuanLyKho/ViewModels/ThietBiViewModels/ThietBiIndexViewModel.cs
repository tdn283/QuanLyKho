using QuanLyKho.Models;
using QuanLyKho.ViewModels.OtherViewModels;

namespace QuanLyKho.ViewModels.ThietBiViewModels
{
    public class ThietBiIndexViewModel
    {
        public List<ThietBiViewModel> ThietBis { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}
