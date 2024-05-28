using QuanLyKho.Models;
namespace QuanLyKho.Data.Interface
{
    public interface INhanVienService
    {
        Task<IEnumerable<TaiKhoan>> GetAllNhanviensAsync();
        Task<TaiKhoan> GetNhanvienByIdAsync(string id);
        Task AddNhanvienAsync(TaiKhoan sinhVien);
        Task<TaiKhoan> UpdateNhanvienAsync(string id, TaiKhoan newNhanVien);
        Task DeleteNhanvienAsync(string id);
    }
}
