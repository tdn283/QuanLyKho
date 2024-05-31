using Microsoft.EntityFrameworkCore;
using QuanLyKho.Data.Interface;
using QuanLyKho.Models;

namespace QuanLyKho.Data.Service
{
    public class DanhMucService : IDanhMucService
    {
        private readonly QuanlykhoContext _context;
        public DanhMucService(QuanlykhoContext context)
        {
            _context = context;
        }
        public async Task AddDanhMucAsync(DanhMucThietBi danhMuc)
        {
            await _context.DanhMucThietBis.AddAsync(danhMuc);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDanhMucAsync(string id)
        {
            var danhMuc = await _context.DanhMucThietBis.FindAsync(id);
            if (danhMuc == null)
            {
                throw new Exception("Không tìm thấy danh mục");
            }
            _context.DanhMucThietBis.Remove(danhMuc);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<DanhMucThietBi>> GetAllDanhMucAsync()
        {
            return await _context.DanhMucThietBis.ToListAsync();
        }

        public async Task<DanhMucThietBi> GetDanhMucIdAsync(string id)
        {
            var danhMuc = await _context.DanhMucThietBis.FindAsync(id);
            if (danhMuc == null)
            {
                throw new Exception("Không tìm thấy danh mục");
            }
            return danhMuc;
        }

        public async Task<DanhMucThietBi> UpdateDanhMucAsync(string id, DanhMucThietBi newDanhMuc)
        {
            _context.DanhMucThietBis.Update(newDanhMuc);
            await _context.SaveChangesAsync();
            return newDanhMuc;
        }
    }
}
