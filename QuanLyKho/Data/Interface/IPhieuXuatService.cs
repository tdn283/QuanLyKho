using QuanLyKho.Models;

namespace QuanLyKho.Data.Interface
{
    public interface IPhieuXuatService
    {
        Task<IEnumerable<PhieuXuatHang>> GetAllPhieuXuatAsync();
        Task<PhieuXuatHang> GetPhieuXuatByIdAsync(string id);
        Task AddPhieuXuatAsync(PhieuXuatHang phieuXuat);
        Task<PhieuXuatHang> UpdatePhieuXuatAsync(string id, PhieuXuatHang newPhieuXuat);
        Task DeletePhieuXuatAsync(string id);
    }
}
