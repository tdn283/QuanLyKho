using Microsoft.EntityFrameworkCore;
using QuanLyKho.Data.Interface;
using QuanLyKho.Models;

namespace QuanLyKho.Data.Service
{
    public class PhieuXuatService : IPhieuXuatService
    {
        private readonly QuanlykhoContext _context;
        public PhieuXuatService(QuanlykhoContext context)
        {
            _context = context;
        }
        public async Task AddPhieuXuatAsync(PhieuXuatHang phieuXuat)
        {
            await _context.PhieuXuatHangs.AddAsync(phieuXuat);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePhieuXuatAsync(string id)
        {
            var phieuXuat = await _context.PhieuXuatHangs.FindAsync(id);
            if (phieuXuat == null)
            {
                throw new Exception("Không tìm thấy phiếu xuất");
            }
            _context.PhieuXuatHangs.Remove(phieuXuat);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PhieuXuatHang>> GetAllPhieuXuatAsync()
        {
            return await _context.PhieuXuatHangs.ToListAsync();
        }

        public async Task<PhieuXuatHang> GetPhieuXuatByIdAsync(string id)
        {
            var phieuXuat = await _context.PhieuXuatHangs.FindAsync(id);
            if (phieuXuat == null)
            {
                throw new Exception("Không tìm thấy phiếu xuất");
            }
            return phieuXuat;
        }

        public async Task<PhieuXuatHang> UpdatePhieuXuatAsync(string id, PhieuXuatHang newPhieuXuat)
        {
            _context.PhieuXuatHangs.Update(newPhieuXuat);
            await _context.SaveChangesAsync();
            return newPhieuXuat;
        }
    }
}
