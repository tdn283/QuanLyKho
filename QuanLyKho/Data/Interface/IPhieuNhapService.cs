using QuanLyKho.Models;

namespace QuanLyKho.Data.Interface
{
    public interface IPhieuNhapService
    {
        Task<IEnumerable<PhieuNhapHang>> GetAllPhieuNhapAsync();
        Task<PhieuNhapHang> GetPhieuNhapByIdAsync(string id);
        Task AddPhieuNhapAsync(PhieuNhapHang phieuNhap);
        Task<PhieuNhapHang> UpdatePhieuNhapAsync(string id, PhieuNhapHang newPhieuNhap);
        Task DeletePhieuNhapAsync(string id);
    }
}
