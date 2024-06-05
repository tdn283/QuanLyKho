using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyKho.Data.Enums;
using QuanLyKho.Data.Interface;
using QuanLyKho.Data.Service;
using QuanLyKho.Helper;
using QuanLyKho.Models;
using QuanLyKho.ViewModels.NhaCungCapViewModel;
using QuanLyKho.ViewModels.OtherViewModels;
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
        public async Task<IActionResult> Index(string? searchString = null, string? trangThaiFilter = null, int pageNumber = 1, int pageSize = 10)
        {
            searchString = string.IsNullOrEmpty(searchString) ? "" : searchString.ToLower();

            var phieuNhapList = await _phieuNhapService.GetAllPhieuNhapAsync();
            var nguoiDungList = await _context.TaiKhoans.ToListAsync();

            // Viewbag
            var trangThaiList = phieuNhapList.Select(pn => pn.TrangThai).Distinct().Select(trangThai => new SelectListItem
            {
                Value = trangThai,
                Text = trangThai,
                Selected = trangThai == trangThaiFilter
            }).ToList();
            ViewBag.TrangThaiList = trangThaiList;


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
                    (string.IsNullOrEmpty(searchString) || pn.MaPhieuNhap.ToLower().Contains(searchString)) && // Search by MaPhieuNhap
                    (trangThaiFilter == null || pn.TrangThai == trangThaiFilter) // Filter by TrangThai
                    )
                    .ToList();

            // Pagination
            int totalItems = phieuNhapVM.Count;
            var pagedPhieuNhapVM = phieuNhapVM.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var phieuNhapIndexVM = new PhieuNhapIndexViewModel
            {
                PhieuNhaps = pagedPhieuNhapVM,
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = pageSize,
                    TotalItems = totalItems
                }
            };

            ViewData["searchString"] = searchString; // Keep the search string after reloading the page
            ViewData["trangThaiFilter"] = trangThaiFilter; // Keep the filter after reloading the page

            return View(phieuNhapIndexVM);
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
                GhiChu = null,
                ThongTinThietBis = thietBiList.Select(tb => new ThongTinThietBiCreateItem
                {
                    MaThietBi = tb.MaThietBi,
                    MaNhaCungCap = tb.MaNhaCungCap,
                    TenNhaCungCap = tb.MaNhaCungCapNavigation.TenNhaCungCap,
                    TenThietBi = tb.TenThietBi,
                    SoLuongCon = tb.SoLuongCon,
                    GiaBan = tb.GiaBan,
                    SoLuongNhap = 0,
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
                .FromSqlRaw("SELECT * FROM ChiTietPhieuNhap WHERE maPhieuNhap = {0}", id)
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

            phieuNhapDetailsVM.TrangThaiList = Enum.GetValues(typeof(TrangThaiPhieuNhap))
                .Cast<TrangThaiPhieuNhap>()
                .Select(value => new SelectListItem
                {
                    Value = EnumHelper.GetDisplayName(value),
                    Text = EnumHelper.GetDisplayName(value),
                }).ToList();

            // Set the text of selected value to the current status
            phieuNhapDetailsVM.SelectedTrangThai = phieuNhap.TrangThai;


            return View(phieuNhapDetailsVM);
        }

        // POST: PhieuNhap/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(string id, string SelectedTrangThai, PhieuNhapDetailsViewModel phieuNhapDetailsVM)
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

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update the status of the order, then update SoLuongCon of each product
                phieuNhap.TrangThai = SelectedTrangThai;
                if (SelectedTrangThai == "Đã nhập kho")
                {
                    // Get the associated ChiTietPhieuNhap records
                    var chiTietPhieuNhapList = await _context.ChiTietPhieuNhaps
                        .FromSqlRaw("SELECT * FROM ChiTietPhieuNhap WHERE maPhieuNhap = {0}", id)
                        .ToListAsync();

                    // Update SoLuongCon for each ThongTinThietBi
                    foreach (var chiTiet in chiTietPhieuNhapList)
                    {
                        var thietBi = await _context.ThongTinThietBis.FindAsync(chiTiet.MaThietBi);
                        if (thietBi != null)
                        {
                            thietBi.SoLuongCon += chiTiet.SoLuongNhap;
                        }
                    }
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();

                return BadRequest("Đã xảy ra lỗi. Vui lòng thử lại sau.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Có lỗi đã xảy ra trong quá trình cập nhật dữ liệu. Xin vui lòng thử lại sau.");
            }
            TempData["SuccessMessage"] = "Cập nhật trạng thái thành công!";
            return RedirectToAction("Details", new { id });
        }

        // GET: PhieuNhap/Delete/5
        public async Task<IActionResult> Delete(string id)
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

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var chiTietPhieuNhap = await _context.ChiTietPhieuNhaps
                    .Where(c => c.MaPhieuNhap == id)
                    .ToListAsync();

                if (chiTietPhieuNhap.Any())
                {
                    _context.ChiTietPhieuNhaps.RemoveRange(chiTietPhieuNhap);
                    await _context.SaveChangesAsync();
                }

                await _phieuNhapService.DeletePhieuNhapAsync(id);

                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();

                return BadRequest("Đã xảy ra lỗi. Vui lòng thử lại sau.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Đã có lỗi xảy ra trong quá trình cập nhật dữ liệu. Vui lòng thử lại sau.");
            }

            return RedirectToAction("Index");
        }

    }
}
