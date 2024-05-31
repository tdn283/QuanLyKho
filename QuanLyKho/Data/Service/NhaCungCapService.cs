using Microsoft.EntityFrameworkCore;
using QuanLyKho.Data.Interface;
using QuanLyKho.Models;

namespace QuanLyKho.Data.Service
{
    public class NhaCungCapService : INhaCungCapService
    {
        private readonly QuanlykhoContext _context;
        public NhaCungCapService(QuanlykhoContext context)
        {
            _context = context;
        }
        public async Task AddNhaCungCapAsync(NhaCungCap nhaCungCap)
        {
            await _context.NhaCungCaps.AddAsync(nhaCungCap);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNhaCungCapAsync(string id)
        {
            var nhaCungCap = await _context.NhaCungCaps.FindAsync(id);
            if (nhaCungCap == null)
            {
                throw new Exception("Không tìm thấy nhà cung cấp");
            }
            _context.NhaCungCaps.Remove(nhaCungCap);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<NhaCungCap>> GetAllNhaCungCapAsync()
        {
            return await _context.NhaCungCaps.ToListAsync();
        }

        public async Task<NhaCungCap> GetNhaCungCapByIdAsync(string id)
        {
            var nhaCungCap = await _context.NhaCungCaps.FindAsync(id);
            if (nhaCungCap == null)
            {
                throw new Exception("Không tìm thấy nhà cung cấp");
            }
            return nhaCungCap;
        }

        public async Task<NhaCungCap> UpdateNhaCungCapAsync(string id, NhaCungCap newNhaCungCap)
        {
            _context.NhaCungCaps.Update(newNhaCungCap);
            await _context.SaveChangesAsync();
            return newNhaCungCap;
        }
    }
}
