using Microsoft.EntityFrameworkCore;
using QuanLyKho.Data.Interface;
using QuanLyKho.Models;

namespace QuanLyKho.Data.Service
{
    public class NhanVienService : INhanVienService
    {
        private readonly QuanlykhoContext _context;
        public NhanVienService(QuanlykhoContext context)
        {
            _context = context;
        }
        public async Task AddNhanvienAsync(TaiKhoan sinhVien)
        {
            await _context.TaiKhoans.AddAsync(sinhVien);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNhanvienAsync(string id)
        {
            var nhanVien = await _context.TaiKhoans.FindAsync(id);
            if (nhanVien == null)
            {
                throw new Exception("Không tìm thấy nhân viên");
            }
            _context.TaiKhoans.Remove(nhanVien);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaiKhoan>> GetAllNhanviensAsync()
        {
            return await _context.TaiKhoans.ToListAsync();
        }

        public async Task<TaiKhoan> GetNhanvienByIdAsync(string id)
        {
            var nhanVien = await _context.TaiKhoans.FindAsync(id);
            if (nhanVien == null)
            {
                throw new Exception("Không tìm thấy nhân viên");
            }
            return nhanVien;
        }

        public async Task<TaiKhoan> UpdateNhanvienAsync(string id, TaiKhoan newNhanVien)
        {
            _context.TaiKhoans.Update(newNhanVien);
            await _context.SaveChangesAsync();
            return newNhanVien;
        }
    }
}
