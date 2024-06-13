using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyKho.Data.Enums;
using QuanLyKho.Data.Interface;
using QuanLyKho.Data.Service;
using QuanLyKho.Helper;
using QuanLyKho.Models;
using QuanLyKho.ViewModels.OtherViewModels;
using QuanLyKho.ViewModels.PhieuNhapViewModels;
using System.Globalization;
using System.Text;

namespace QuanLyKho.Controllers
{
    [Authorize]
    public class PhieuNhapController : Controller
    {
        private readonly QuanlykhoContext _context;
        private readonly IPhieuNhapService _phieuNhapService;
        public PhieuNhapController(QuanlykhoContext context, IPhieuNhapService phieuNhapService)
        {
            _context = context;
            _phieuNhapService = phieuNhapService;
        }
        // GET: PhieuNhap/Index
        public async Task<IActionResult> Index(string searchString = "", string trangThaiFilter = null, string sortBy = null, int pageNumber = 1, int pageSize = 10)
        {
            // Normalize search string (lowercase, case-insensitive)
            searchString = string.IsNullOrEmpty(searchString) ? "" : searchString.ToLower();

            // Fetch all PhieuNhap and NguoiDung entities
            var phieuNhapList = await _phieuNhapService.GetAllPhieuNhapAsync();
            var nguoiDungList = await _context.TaiKhoans.ToListAsync();

            // Populate ViewBag with TrangThai filter options
            ViewBag.TrangThaiList = phieuNhapList
                .Select(pn => pn.TrangThai)
                .Distinct()
                .Select(trangThai => new SelectListItem
                {
                    Value = trangThai,
                    Text = trangThai,
                    Selected = trangThai == trangThaiFilter
                })
                .ToList();

            // Map PhieuNhap entities to view models, including related NguoiDung information
            var phieuNhapVM = phieuNhapList
                .Select(pn =>
                {
                    var nguoiDung = nguoiDungList.FirstOrDefault(nd => nd.MaNguoiDung == pn.MaNguoiDung);
                    return new PhieuNhapViewModel
                    {
                        MaPhieuNhap = pn.MaPhieuNhap,
                        MaNguoiDung = pn.MaNguoiDung,
                        HoVaTen = nguoiDung?.HoVaTen ?? "", // Handle null nguoiDung
                        NgayNhap = pn.NgayNhap.ToString("dd/MM/yyyy"),
                        TrangThai = pn.TrangThai,
                        TongTien = pn.TongTien,
                        GhiChu = pn.GhiChu
                    };
                })
                // Filter based on search string and status
                .Where(pn =>
                    (string.IsNullOrEmpty(searchString) || pn.MaPhieuNhap.ToLower().Contains(searchString)) &&
                    (trangThaiFilter == null || pn.TrangThai == trangThaiFilter)
                )
                .ToList();

            // Sort the results based on the 'sortBy' parameter
            phieuNhapVM = sortBy switch
            {
                "ngayNhap_asc" => phieuNhapVM.OrderBy(pn => DateTime.ParseExact(pn.NgayNhap, "dd-MM-yyyy", CultureInfo.InvariantCulture)).ToList(),
                "ngayNhap_desc" => phieuNhapVM.OrderByDescending(pn => DateTime.ParseExact(pn.NgayNhap, "dd-MM-yyyy", CultureInfo.InvariantCulture)).ToList(),
                "tongTien_asc" => phieuNhapVM.OrderBy(pn => pn.TongTien).ToList(),
                "tongTien_desc" => phieuNhapVM.OrderByDescending(pn => pn.TongTien).ToList(),
                _ => phieuNhapVM // Default: no sorting
            };

            // Apply pagination to the filtered and sorted results
            int totalItems = phieuNhapVM.Count;
            var pagedPhieuNhapVM = phieuNhapVM.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Create the view model for the Index view, combining data and pagination info
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

            // Store parameters in ViewData to persist them after page reloads
            ViewData["searchString"] = searchString;
            ViewData["trangThaiFilter"] = trangThaiFilter;
            ViewData["sortBy"] = sortBy;

            // Return the Index view with the constructed view model
            return View(phieuNhapIndexVM);
        }


        // GET: PhieuNhap/Create
        public async Task<IActionResult> Create(string maNguoiDung)
        {
            // Validate the maNguoiDung input
            if (string.IsNullOrEmpty(maNguoiDung))
            {
                return NotFound(); // Return 404 if input is invalid
            }

            // Fetch all ThietBi and their NhaCungCap
            var thietBiList = await _context.ThongTinThietBis.Include(tb => tb.MaNhaCungCapNavigation).ToListAsync();

            // Fetch the NguoiDung associated with the given maNguoiDung
            var nguoiDung = await _context.TaiKhoans.FirstOrDefaultAsync(nd => nd.MaNguoiDung == maNguoiDung);
            if (nguoiDung == null)
            {
                return NotFound(); // Return 404 if user not found
            }

            // Create the PhieuNhapCreateViewModel and populate it with initial data
            var phieuNhapCreateVM = new PhieuNhapCreateViewModel()
            {
                MaNguoiDung = nguoiDung.MaNguoiDung,
                HoVaTen = nguoiDung.HoVaTen,
                TongTien = 0,
                GhiChu = null,
                // Map ThietBi to ThongTinThietBiCreateItem view models
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

            // Return the Create view with the view model
            return View(phieuNhapCreateVM);
        }

        // POST: PhieuNhap/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string maNguoiDung, PhieuNhapCreateViewModel phieuNhapCreateVM)
        {
            // Validate model state
            if (ModelState.IsValid)
            {
                // Start a database transaction to ensure data consistency
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    // Create the PhieuNhapHang entity
                    var phieuNhap = new PhieuNhapHang
                    {
                        MaPhieuNhap = AutoIncrementHelper.TaoMaPhieuNhapMoi(_context), // Generate a new ID
                        MaNguoiDung = phieuNhapCreateVM.MaNguoiDung,
                        NgayNhap = DateTime.Now,
                        TongTien = phieuNhapCreateVM.TongTien,
                        GhiChu = phieuNhapCreateVM.GhiChu,
                        TrangThai = EnumHelper.GetDisplayName(TrangThaiPhieuNhap.ChoNhapKho).Normalize(NormalizationForm.FormC)
                    };

                    // Add PhieuNhap to the database
                    await _phieuNhapService.AddPhieuNhapAsync(phieuNhap);

                    // Add ChiTietPhieuNhap for each item with a quantity > 0
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

                    // Save changes and commit the transaction
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return RedirectToAction("Index");
                }
                // Error handling
                catch (Exception)
                {
                    await transaction.RollbackAsync(); // Rollback on error
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong lúc tạo phiếu nhập.");
                }
            }

            // If model state is invalid, return the view with the model
            return View(phieuNhapCreateVM);
        }


        // GET: PhieuNhap/Details/5
        public async Task<IActionResult> Details(string id)
        {
            // Validate the ID
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Fetch the PhieuNhap
            var phieuNhap = await _phieuNhapService.GetPhieuNhapByIdAsync(id);
            if (phieuNhap == null)
            {
                return NotFound();
            }

            // Fetch all ThietBi and their NhaCungCap
            var thongTinThietBiList = await _context.ThongTinThietBis.Include(tb => tb.MaNhaCungCapNavigation).ToListAsync();
            if (thongTinThietBiList == null)
            {
                return NotFound();
            }

            // Fetch ChiTietPhieuNhap for this PhieuNhap
            var chiTietPhieuNhapList = await _context.ChiTietPhieuNhaps
                .FromSqlRaw("SELECT * FROM ChiTietPhieuNhap WHERE maPhieuNhap = {0}", id)
                .ToListAsync();

            // Map ChiTietPhieuNhap to ThongTinThietBiDetailsItem view models
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

            // Map PhieuNhap to PhieuNhapDetailsViewModel, including related data
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

            // Populate TrangThaiList with status options for the dropdown
            phieuNhapDetailsVM.TrangThaiList = Enum.GetValues(typeof(TrangThaiPhieuNhap))
                .Cast<TrangThaiPhieuNhap>()
                .Select(value => new SelectListItem
                {
                    Value = EnumHelper.GetDisplayName(value),
                    Text = EnumHelper.GetDisplayName(value),
                }).ToList();

            // Set the currently selected status in the dropdown
            phieuNhapDetailsVM.SelectedTrangThai = phieuNhap.TrangThai;

            // Return the view with the model
            return View(phieuNhapDetailsVM);
        }


        // POST: PhieuNhap/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(string id, string SelectedTrangThai, PhieuNhapDetailsViewModel phieuNhapDetailsVM)
        {
            // Input validation
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Fetch the existing PhieuNhap
            var phieuNhap = await _phieuNhapService.GetPhieuNhapByIdAsync(id);
            if (phieuNhap == null)
            {
                return NotFound();
            }

            // Use a transaction for data consistency
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update PhieuNhap status
                phieuNhap.TrangThai = SelectedTrangThai;

                // If status is "Đã nhập kho", update inventory levels
                if (SelectedTrangThai == "Đã nhập kho")
                {
                    var chiTietPhieuNhapList = await _context.ChiTietPhieuNhaps
                        .FromSqlRaw("SELECT * FROM ChiTietPhieuNhap WHERE maPhieuNhap = {0}", id)
                        .ToListAsync();

                    foreach (var chiTiet in chiTietPhieuNhapList)
                    {
                        var thietBi = await _context.ThongTinThietBis.FindAsync(chiTiet.MaThietBi);
                        if (thietBi != null)
                        {
                            thietBi.SoLuongCon += chiTiet.SoLuongNhap;
                        }
                    }
                }

                await _context.SaveChangesAsync(); // Save changes
                await transaction.CommitAsync();   // Commit the transaction
            }
            // Catch and handle potential errors
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(); // Rollback if concurrency error
                return BadRequest("Đã xảy ra lỗi. Vui lòng thử lại sau.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Rollback on any other error
                return StatusCode(500, "Có lỗi đã xảy ra trong quá trình cập nhật dữ liệu. Xin vui lòng thử lại sau.");
            }

            // Set success message in TempData
            TempData["SuccessMessage"] = "Cập nhật trạng thái thành công!";

            // Redirect back to the Details view
            return RedirectToAction("Details", new { id });
        }

        // GET: PhieuNhap/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            // Validate the ID
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Fetch the PhieuNhap
            var phieuNhap = await _phieuNhapService.GetPhieuNhapByIdAsync(id);
            if (phieuNhap == null)
            {
                return NotFound();
            }

            // Begin a database transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Fetch and delete associated ChiTietPhieuNhap records
                var chiTietPhieuNhap = await _context.ChiTietPhieuNhaps
                    .Where(c => c.MaPhieuNhap == id)
                    .ToListAsync();
                if (chiTietPhieuNhap.Any())
                {
                    _context.ChiTietPhieuNhaps.RemoveRange(chiTietPhieuNhap);
                    await _context.SaveChangesAsync();
                }

                // Delete the PhieuNhap itself
                await _phieuNhapService.DeletePhieuNhapAsync(id);

                // Commit the transaction if successful
                await transaction.CommitAsync();
            }
            // Handle concurrency issues
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                return BadRequest("Đã xảy ra lỗi. Vui lòng thử lại sau.");
            }
            // Handle other exceptions
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Đã có lỗi xảy ra trong quá trình cập nhật dữ liệu. Vui lòng thử lại sau.");
            }

            // Redirect to Index after deletion
            return RedirectToAction("Index");
        }

    }
}
