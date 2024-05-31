using QuanLyKho.Models;

namespace QuanLyKho.Data.Interface
{
    public interface IDanhMucService
    {
        Task<IEnumerable<DanhMucThietBi>> GetAllDanhMucAsync();
        Task<DanhMucThietBi> GetDanhMucIdAsync(string id);
        Task AddDanhMucAsync(DanhMucThietBi danhMuc);
        Task<DanhMucThietBi> UpdateDanhMucAsync(string id, DanhMucThietBi newDanhMuc);
        Task DeleteDanhMucAsync(string id);
    }
}
