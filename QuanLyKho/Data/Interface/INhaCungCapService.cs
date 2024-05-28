using QuanLyKho.Models;

namespace QuanLyKho.Data.Interface
{
    public interface INhaCungCapService
    {
        Task<IEnumerable<NhaCungCap>> GetAllNhaCungCapAsync();
        Task<NhaCungCap> GetNhaCungCapByIdAsync(string id);
        Task AddNhaCungCapAsync(NhaCungCap nhaCungCap);
        Task<NhaCungCap> UpdateNhaCungCapAsync(string id, NhaCungCap newNhaCungCap);
        Task DeleteNhaCungCapAsync(string id);
    }
}
