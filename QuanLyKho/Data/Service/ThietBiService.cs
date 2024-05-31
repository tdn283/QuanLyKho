using Microsoft.EntityFrameworkCore;
using QuanLyKho.Data.Interface;
using QuanLyKho.Models;

namespace QuanLyKho.Data.Service
{
    public class ThietBiService : IThietBiService
    {
        private readonly QuanlykhoContext _context;
        public ThietBiService(QuanlykhoContext context)
        {
            _context = context;
        }
        public async Task AddThietBiAsync(ThongTinThietBi thietBi)
        {
            await _context.ThongTinThietBis.AddAsync(thietBi);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteThietBiAsync(string id)
        {
            var thietBi = await _context.ThongTinThietBis.FindAsync(id);
            if (thietBi == null)
            {
                throw new Exception("Không tìm thấy thiết bị");
            }
            _context.ThongTinThietBis.Remove(thietBi);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ThongTinThietBi>> GetAllThietBiAsync()
        {
            return await _context.ThongTinThietBis.ToListAsync();
        }

        public async Task<ThongTinThietBi> GetThietBiByIdAsync(string id)
        {
            var thietBi = await _context.ThongTinThietBis.FindAsync(id);
            if (thietBi == null)
            {
                throw new Exception("Không tìm thấy thiết bị");
            }
            return thietBi;
        }

        public async Task<ThongTinThietBi> UpdateThietBiAsync(string id, ThongTinThietBi newThietBi)
        {
            _context.ThongTinThietBis.Update(newThietBi);
            await _context.SaveChangesAsync();
            return newThietBi;
        }
    }
}
