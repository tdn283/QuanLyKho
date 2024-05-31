using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyKho.Data.Enums;
using QuanLyKho.Data.Interface;
using QuanLyKho.Helper;
using QuanLyKho.Models;
using QuanLyKho.ViewModels.PhieuNhapViewModels;
using System.Text;

namespace QuanLyKho.Controllers
{
    public class PhieuNhapController : Controller
    {
        private readonly QuanlykhoContext _context;
        private readonly IPhieuNhapService _phieuNhapService;
        public PhieuNhapController(QuanlykhoContext context, IPhieuNhapService phieuNhapService)
        {
            _context = context;
            _phieuNhapService = phieuNhapService;
        }
        public async Task<IActionResult> Index(string? searchString = null, string? trangThaiFilter = null)
        {
            var phieuNhapList = await _phieuNhapService.GetAllPhieuNhapAsync();
            var nguoiDungList = await _context.TaiKhoans.ToListAsync();

            var trangThaiList = phieuNhapList.Select(pn => new SelectListItem
            {
                Value = pn.TrangThai,
                Text = pn.TrangThai,
                Selected = pn.TrangThai == trangThaiFilter
            }).ToList();

            ViewBag.TrangThaiList = trangThaiList;

            searchString = string.IsNullOrEmpty(searchString) ? "" : searchString.ToLower();

            var phieuNhapVM = phieuNhapList
                .Select(pn =>
                {
                    var nguoiDung = nguoiDungList.FirstOrDefault(nd => nd.MaNguoiDung == pn.MaNguoiDung);
                    return new PhieuNhapViewModel
                    {
                        MaPhieuNhap = pn.MaPhieuNhap,
                        MaNguoiDung = pn.MaNguoiDung,
                        HoVaTen = nguoiDung?.HoVaTen ?? "",
                        NgayNhap = pn.NgayNhap.ToString("dd/MM/yyyy"),
                        TrangThai = pn.TrangThai,
                        TongTien = pn.TongTien,
                        GhiChu = pn.GhiChu
                    };
                })
                .Where(pn =>
                    (string.IsNullOrEmpty(searchString) || pn.MaPhieuNhap.ToLower().Contains(searchString)) &&
                    (trangThaiFilter == null || pn.TrangThai == trangThaiFilter)
                    )
                    .ToList();
            return View(phieuNhapVM);
        }

        // GET: PhieuNhap/Create
        public async Task<IActionResult> Create(string maNguoiDung)
        {
            if (string.IsNullOrEmpty(maNguoiDung))
            {
                return RedirectToAction("Index");
            }
            var thietBiList = await _context.ThongTinThietBis.Include(tb => tb.MaNhaCungCapNavigation).ToListAsync();
            var nguoiDung = await _context.TaiKhoans.FirstOrDefaultAsync(nd => nd.MaNguoiDung == maNguoiDung);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            var phieuNhapCreateVM = new PhieuNhapCreateViewModel()
            {
                MaNguoiDung = nguoiDung.MaNguoiDung,
                HoVaTen = nguoiDung.HoVaTen,
                TongTien = 0,
                ThongTinThietBis = thietBiList.Select(tb => new ThongTinThietBiCreateItem
                {
                    MaThietBi = tb.MaThietBi,
                    MaNhaCungCap = tb.MaNhaCungCap,
                    TenNhaCungCap = tb.MaNhaCungCapNavigation.TenNhaCungCap,
                    TenThietBi = tb.TenThietBi,
                    SoLuongCon = tb.SoLuongCon,
                    GiaBan = tb.GiaBan,
                    SoLuongNhap = 0
                }).ToList()
            };

            return View(phieuNhapCreateVM);
        }

        // POST: PhieuNhap/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string maNguoiDung, PhieuNhapCreateViewModel phieuNhapCreateVM)
        {
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    var phieuNhap = new PhieuNhapHang();
                    phieuNhap.MaPhieuNhap = AutoIncrementHelper.TaoMaPhieuNhapMoi(_context);
                    phieuNhap.MaNguoiDung = phieuNhapCreateVM.MaNguoiDung;
                    phieuNhap.NgayNhap = DateTime.Now;
                    phieuNhap.TongTien = phieuNhapCreateVM.TongTien;
                    phieuNhap.GhiChu = phieuNhapCreateVM.GhiChu?.Normalize(NormalizationForm.FormC);
                    phieuNhap.TrangThai = EnumHelper.GetDisplayName(TrangThaiPhieuNhap.ChoNhapKho).Normalize(NormalizationForm.FormC);



                    await _phieuNhapService.AddPhieuNhapAsync(phieuNhap);

                    foreach (var item in phieuNhapCreateVM.ThongTinThietBis)
                    {
                        if (item.SoLuongNhap > 0)
                        {
                            var parameters = new List<SqlParameter>();
                            parameters.Add(new SqlParameter("@maPhieuNhap", phieuNhap.MaPhieuNhap));
                            parameters.Add(new SqlParameter("@maThietBi", item.MaThietBi));
                            parameters.Add(new SqlParameter("@soLuongNhap", item.SoLuongNhap));
                            parameters.Add(new SqlParameter("@donGiaNhap", item.GiaBan * 0.9m));
                            await _context.Database.ExecuteSqlRawAsync(
                                "EXEC ThemChiTietPhieuNhap @maPhieuNhap, @maThietBi, @soLuongNhap, @donGiaNhap",
                                parameters);
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return RedirectToAction("Index");

                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong lúc tạo phiếu nhập.");
                }
            }
            return View(phieuNhapCreateVM);
        }

        // GET: PhieuNhap/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var phieuNhap = await _phieuNhapService.GetPhieuNhapByIdAsync(id);
            if (phieuNhap == null)
            {
                return NotFound();
            }

            var thongTinThietBiList = await _context.ThongTinThietBis.Include(tb => tb.MaNhaCungCapNavigation).ToListAsync();
            if (thongTinThietBiList == null)
            {
                return NotFound();
            }

            var chiTietPhieuNhapList = await _context.ChiTietPhieuNhaps
                .Include(tb => tb.MaThietBiNavigation)
                .Where(ctpn => ctpn.MaPhieuNhap == phieuNhap.MaPhieuNhap)
                .ToListAsync();

            var thietBiDetailsItem = chiTietPhieuNhapList.Select(ctpn =>
            {
                var thietBi = thongTinThietBiList.FirstOrDefault(tb => tb.MaThietBi == ctpn.MaThietBi);
                return new ThongTinThietBiDetailsItem
                {
                    MaThietBi = ctpn.MaThietBi,
                    TenThietBi = thietBi.TenThietBi,
                    MaNhaCungCap = thietBi.MaNhaCungCap,
                    TenNhaCungCap = thietBi.MaNhaCungCapNavigation.TenNhaCungCap,
                    SoLuongCon = thietBi.SoLuongCon,
                    GiaBan = thietBi.GiaBan,
                    SoLuongNhap = ctpn.SoLuongNhap
                };
            }).ToList();

            var phieuNhapDetailsVM = new PhieuNhapDetailsViewModel
            {
                MaPhieuNhap = phieuNhap.MaPhieuNhap,
                MaNguoiDung = phieuNhap.MaNguoiDung,
                HoVaTen = _context.TaiKhoans.FirstOrDefault(nd => nd.MaNguoiDung == phieuNhap.MaNguoiDung)?.HoVaTen,
                NgayNhap = phieuNhap.NgayNhap.ToString("dd/MM/yyyy"),
                TrangThai = phieuNhap.TrangThai,
                TongTien = phieuNhap.TongTien,
                GhiChu = phieuNhap.GhiChu,
                ThongTinThietBis = thietBiDetailsItem
            };
            return View(phieuNhapDetailsVM);
        }
    }
}
