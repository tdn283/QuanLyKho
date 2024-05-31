using Microsoft.EntityFrameworkCore;
using QuanLyKho.Data.Interface;
using QuanLyKho.Models;

namespace QuanLyKho.Data.Service
{
    public class PhieuNhapService : IPhieuNhapService
    {
        private readonly QuanlykhoContext _context;
        public PhieuNhapService(QuanlykhoContext context)
        {
            _context = context;
        }
        public async Task AddPhieuNhapAsync(PhieuNhapHang phieuNhap)
        {
            await _context.PhieuNhapHangs.AddAsync(phieuNhap);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePhieuNhapAsync(string id)
        {
            var phieuNhap = await _context.PhieuNhapHangs.FindAsync(id);
            if (phieuNhap == null)
            {
                throw new Exception("Không tìm thấy phiếu nhập");
            }
            _context.PhieuNhapHangs.Remove(phieuNhap);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PhieuNhapHang>> GetAllPhieuNhapAsync()
        {
            return await _context.PhieuNhapHangs.ToListAsync();
        }

        public async Task<PhieuNhapHang> GetPhieuNhapByIdAsync(string id)
        {
            var phieuNhap = await _context.PhieuNhapHangs.FindAsync(id);
            if (phieuNhap == null)
            {
                throw new Exception("Không tìm thấy phiếu nhập");
            }
            return phieuNhap;
        }

        public async Task<PhieuNhapHang> UpdatePhieuNhapAsync(string id, PhieuNhapHang newPhieuNhap)
        {
            _context.PhieuNhapHangs.Update(newPhieuNhap);
            await _context.SaveChangesAsync();
            return newPhieuNhap;
        }
    }
}
