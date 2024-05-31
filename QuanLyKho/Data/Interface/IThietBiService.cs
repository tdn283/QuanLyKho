using QuanLyKho.Models;

namespace QuanLyKho.Data.Interface
{
    public interface IThietBiService
    {
        Task<IEnumerable<ThongTinThietBi>> GetAllThietBiAsync();
        Task<ThongTinThietBi> GetThietBiByIdAsync(string id);
        Task AddThietBiAsync(ThongTinThietBi thietBi);
        Task<ThongTinThietBi> UpdateThietBiAsync(string id, ThongTinThietBi newThietBi);
        Task DeleteThietBiAsync(string id);
    }
}
