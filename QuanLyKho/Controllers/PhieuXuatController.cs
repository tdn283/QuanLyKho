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
using QuanLyKho.ViewModels.PhieuXuatViewModels;
using System.Globalization;
using System.Text;

namespace QuanLyKho.Controllers
{
    [Authorize]
    public class PhieuXuatController : Controller
    {
        private readonly QuanlykhoContext _context;
        private readonly IPhieuXuatService _phieuXuatService;
        public PhieuXuatController(QuanlykhoContext context, IPhieuXuatService phieuXuatService)
        {
            _context = context;
            _phieuXuatService = phieuXuatService;
        }

        // GET: PhieuXuat/Index
        public async Task<IActionResult> Index(string searchString = "", string trangThaiFilter = null, string sortBy = null, int pageNumber = 1, int pageSize = 10)
        {
            // Fetch all PhieuXuat and NguoiDung entities
            var phieuXuatList = await _phieuXuatService.GetAllPhieuXuatAsync();
            var nguoiDungList = await _context.TaiKhoans.ToListAsync();

            // Populate ViewBag with TrangThai filter options
            ViewBag.TrangThaiList = phieuXuatList
                .Select(px => px.TrangThai)
                .Distinct()
                .Select(trangThai => new SelectListItem
                {
                    Value = trangThai,
                    Text = trangThai,
                    Selected = trangThai == trangThaiFilter
                })
                .ToList();

            // Normalize search string (lowercase, case-insensitive)
            searchString = string.IsNullOrEmpty(searchString) ? "" : searchString.ToLower();

            // Map PhieuXuat entities to view models, including related NguoiDung info
            var phieuXuatVM = phieuXuatList
                .Select(px =>
                {
                    var nguoiDung = nguoiDungList.FirstOrDefault(nd => nd.MaNguoiDung == px.MaNguoiDung);
                    return new PhieuXuatViewModel
                    {
                        MaPhieuXuat = px.MaPhieuXuat,
                        MaNguoiDung = px.MaNguoiDung,
                        HoVaTen = nguoiDung?.HoVaTen ?? "",
                        NguoiYeuCau = px.NhanVienXuat,
                        NgayXuat = px.NgayXuat.ToString("dd/MM/yyyy"),
                        TrangThai = px.TrangThai,
                        TongTien = px.TongTien,
                        GhiChu = px.GhiChu
                    };
                })
                // Filter based on search string and status
                .Where(px =>
                    (string.IsNullOrEmpty(searchString) || px.MaPhieuXuat.ToLower().Contains(searchString)) &&
                    (trangThaiFilter == null || px.TrangThai == trangThaiFilter)
                )
                .ToList();

            // Sort the results based on the 'sortBy' parameter
            phieuXuatVM = sortBy switch
            {
                "ngayXuat_asc" => phieuXuatVM.OrderBy(px => DateTime.ParseExact(px.NgayXuat, "dd-MM-yyyy", CultureInfo.InvariantCulture)).ToList(),
                "ngayXuat_desc" => phieuXuatVM.OrderByDescending(px => DateTime.ParseExact(px.NgayXuat, "dd-MM-yyyy", CultureInfo.InvariantCulture)).ToList(),
                "tongTien_asc" => phieuXuatVM.OrderBy(px => px.TongTien).ToList(),
                "tongTien_desc" => phieuXuatVM.OrderByDescending(px => px.TongTien).ToList(),
                _ => phieuXuatVM // Default: no sorting
            };

            // Apply pagination to the filtered and sorted results
            int totalItems = phieuXuatVM.Count;
            var pagedPhieuXuatVM = phieuXuatVM.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Create the view model for the Index view, combining data and pagination info
            var phieuXuatIndexVM = new PhieuXuatIndexViewModel
            {
                PhieuXuats = pagedPhieuXuatVM,
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
            return View(phieuXuatIndexVM);
        }


        // GET: PhieuXuat/Create
        public async Task<IActionResult> Create(string maNguoiDung)
        {
            // Validate the maNguoiDung input
            if (string.IsNullOrEmpty(maNguoiDung))
            {
                return NotFound(); // Return 404 if maNguoiDung is not provided
            }

            // Fetch all NhanVien and prepare for dropdown list
            var nhanVienList = _context.TaiKhoans
                .Select(nv => new { MaNv = nv.MaNguoiDung, TenVn = nv.HoVaTen })
                .ToList();
            ViewBag.NhanVienList = nhanVienList.Select(nv => new SelectListItem { Value = nv.TenVn, Text = nv.TenVn });

            // Fetch all ThietBi with their NhaCungCap
            var thietBiList = await _context.ThongTinThietBis
                .Include(tb => tb.MaNhaCungCapNavigation)
                .ToListAsync();

            // Fetch the NguoiDung for the given maNguoiDung
            var nguoiDung = await _context.TaiKhoans.FirstOrDefaultAsync(nd => nd.MaNguoiDung == maNguoiDung);
            if (nguoiDung == null)
            {
                return NotFound(); // Return 404 if user not found
            }

            // Create PhieuXuatCreateViewModel with initial data
            var phieuXuatCreateVM = new PhieuXuatCreateViewModel
            {
                MaNguoiDung = maNguoiDung,
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
                    SoLuongXuat = 0
                }).ToList()
            };

            // Return the view with the model
            return View(phieuXuatCreateVM);
        }

        // POST: PhieuXuat/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string maNguoiDung, PhieuXuatCreateViewModel phieuXuatCreateVM)
        {
            // Re-fetch NhanVien list (in case of errors, to preserve selection)
            var nhanVienList = _context.TaiKhoans.Select(nv => new { MaNv = nv.MaNguoiDung, TenVn = nv.HoVaTen }).ToList();
            ViewBag.NhanVienList = nhanVienList.Select(nv => new SelectListItem { Value = nv.TenVn, Text = nv.TenVn });

            // Validate SoLuongXuat for each item
            foreach (var item in phieuXuatCreateVM.ThongTinThietBis)
            {
                if (item.SoLuongXuat < 0 || item.SoLuongXuat > item.SoLuongCon)
                {
                    ModelState.AddModelError("SoLuongXuat", "Số lượng xuất không hợp lệ.");
                    return View(phieuXuatCreateVM);
                }
            }

            // If model is valid and NguoiYeuCau is selected
            if (ModelState.IsValid && !string.IsNullOrEmpty(phieuXuatCreateVM.NguoiYeuCau))
            {
                // Start database transaction
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    // Create PhieuXuatHang entity
                    var phieuXuat = new PhieuXuatHang
                    {
                        MaPhieuXuat = AutoIncrementHelper.TaoMaPhieuXuatMoi(_context),
                        MaNguoiDung = phieuXuatCreateVM.MaNguoiDung,
                        NhanVienXuat = phieuXuatCreateVM.NguoiYeuCau?.Normalize(NormalizationForm.FormC),
                        NgayXuat = DateTime.Now,
                        TongTien = phieuXuatCreateVM.TongTien,
                        GhiChu = phieuXuatCreateVM.GhiChu?.Normalize(NormalizationForm.FormC),
                        TrangThai = EnumHelper.GetDisplayName(TrangThaiPhieuXuat.ChoDuyet).Normalize(NormalizationForm.FormC)
                    };

                    // Add PhieuXuat and ChiTietPhieuXuat
                    await _phieuXuatService.AddPhieuXuatAsync(phieuXuat);
                    foreach (var item in phieuXuatCreateVM.ThongTinThietBis)
                    {
                        if (item.SoLuongXuat > 0)
                        {
                            var parameters = new List<SqlParameter>();
                            parameters.Add(new SqlParameter("@maPhieuXuat", phieuXuat.MaPhieuXuat));
                            parameters.Add(new SqlParameter("@maThietBi", item.MaThietBi));
                            parameters.Add(new SqlParameter("@soLuongXuat", item.SoLuongXuat));
                            parameters.Add(new SqlParameter("@donGiaBan", item.GiaBan));
                            await _context.Database.ExecuteSqlRawAsync(
                                "EXEC ThemChiTietPhieuXuat @maPhieuXuat, @maThietBi, @soLuongXuat, @donGiaBan",
                                parameters);
                        }
                    }

                    // Save changes and commit transaction
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return RedirectToAction("Index");
                }
                // Error handling
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong lúc tạo phiếu xuất.");
                }
            }
            else
            {
                // If NguoiYeuCau is not selected
                if (string.IsNullOrEmpty(phieuXuatCreateVM.NguoiYeuCau))
                {
                    ModelState.AddModelError("NguoiYeuCau", "Vui lòng chọn nhân viên yêu cầu.");
                }

                // Re-fetch ThietBi list to repopulate in case of errors
                var thietBiList = await _context.ThongTinThietBis
                    .Include(tb => tb.MaNhaCungCapNavigation)
                    .ToListAsync();
                phieuXuatCreateVM.ThongTinThietBis = thietBiList.Select(tb => new ThongTinThietBiCreateItem
                {
                    MaThietBi = tb.MaThietBi,
                    MaNhaCungCap = tb.MaNhaCungCap,
                    TenNhaCungCap = tb.MaNhaCungCapNavigation.TenNhaCungCap,
                    TenThietBi = tb.TenThietBi,
                    SoLuongCon = tb.SoLuongCon,
                    GiaBan = tb.GiaBan,
                    SoLuongXuat = 0
                }).ToList();
            }

            // Return the view with the model to display errors
            return View(phieuXuatCreateVM);
        }


        // GET: PhieuXuat/Details/5
        public async Task<IActionResult> Details(string id)
        {
            // Validate ID
            if (string.IsNullOrEmpty(id))
                return NotFound();

            // Fetch PhieuXuat
            var phieuXuat = await _phieuXuatService.GetPhieuXuatByIdAsync(id);
            if (phieuXuat == null)
                return NotFound();

            // Fetch related ThietBis
            var thongTinThietBiList = await _context.ThongTinThietBis
                .Include(tb => tb.MaNhaCungCapNavigation) // Include supplier details
                .ToListAsync();

            if (thongTinThietBiList == null)
                return NotFound();

            // Fetch ChiTietPhieuXuat using raw SQL
            var chiTietPhieuXuatList = await _context.ChiTietPhieuXuats
                .FromSqlRaw("SELECT * FROM ChiTietPhieuXuat WHERE maPhieuXuat = {0}", id)
                .ToListAsync();

            // Map ChiTietPhieuXuat to ThongTinThietBiDetailsItem view models
            var thietBiDetailsItem = chiTietPhieuXuatList.Select(ctpx =>
            {
                var thietBi = thongTinThietBiList.FirstOrDefault(tb => tb.MaThietBi == ctpx.MaThietBi);
                return new ThongTinThietBiDetailsItem
                {
                    MaThietBi = ctpx.MaThietBi,
                    TenThietBi = thietBi.TenThietBi,
                    MaNhaCungCap = thietBi.MaNhaCungCap,
                    TenNhaCungCap = thietBi.MaNhaCungCapNavigation.TenNhaCungCap,
                    SoLuongCon = thietBi.SoLuongCon,
                    GiaBan = thietBi.GiaBan,
                    SoLuongXuat = ctpx.SoLuong
                };
            }).ToList();

            // Map PhieuXuat to PhieuXuatDetailsViewModel
            var phieuXuatDetailsVM = new PhieuXuatDetailsViewModel
            {
                MaPhieuXuat = phieuXuat.MaPhieuXuat,
                MaNguoiDung = phieuXuat.MaNguoiDung,
                HoVaTen = _context.TaiKhoans.FirstOrDefault(nd => nd.MaNguoiDung == phieuXuat.MaNguoiDung)?.HoVaTen,
                NguoiYeuCau = phieuXuat.NhanVienXuat,
                NgayXuat = phieuXuat.NgayXuat.ToString("dd/MM/yyyy"),
                TrangThai = phieuXuat.TrangThai,
                TongTien = phieuXuat.TongTien,
                GhiChu = phieuXuat.GhiChu,
                ThongTinThietBis = thietBiDetailsItem
            };

            // Populate status dropdown list
            phieuXuatDetailsVM.TrangThaiList = Enum.GetValues(typeof(TrangThaiPhieuXuat))
                .Cast<TrangThaiPhieuXuat>()
                .Select(value => new SelectListItem
                {
                    Value = EnumHelper.GetDisplayName(value),
                    Text = EnumHelper.GetDisplayName(value),
                }).ToList();

            // Set selected status
            phieuXuatDetailsVM.SelectedTrangThai = phieuXuat.TrangThai;

            // Return the view with the model
            return View(phieuXuatDetailsVM);
        }


        // POST: PhieuXuat/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(string id, string SelectedTrangThai, PhieuXuatDetailsViewModel phieuXuatDetailsVM)
        {
            // Input validation
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var phieuXuat = await _phieuXuatService.GetPhieuXuatByIdAsync(id);
            if (phieuXuat == null)
                return NotFound();

            // Begin transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update PhieuXuat status
                phieuXuat.TrangThai = SelectedTrangThai;

                // If status is "Đã duyệt", update inventory levels
                if (SelectedTrangThai == "Đã duyệt")
                {
                    // Fetch related ChiTietPhieuXuat
                    var chiTietPhieuXuatList = await _context.ChiTietPhieuXuats
                        .FromSqlRaw("SELECT * FROM ChiTietPhieuXuat WHERE maPhieuXuat = {0}", id)
                        .ToListAsync();

                    // Update inventory levels for each ThietBi
                    foreach (var chiTiet in chiTietPhieuXuatList)
                    {
                        var thietBi = await _context.ThongTinThietBis.FindAsync(chiTiet.MaThietBi);
                        if (thietBi != null && (thietBi.SoLuongCon - chiTiet.SoLuong >= 0)) // Check if enough stock
                        {
                            thietBi.SoLuongCon -= chiTiet.SoLuong;
                        }
                        else
                        {
                            // Handle insufficient stock
                            throw new Exception("Số lượng không đủ để xuất.");
                        }
                    }
                }

                // Save changes and commit transaction
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            // Handle concurrency issues
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                return BadRequest("Đã xảy ra lỗi. Vui lòng thử lại sau.");
            }
            // Handle general exceptions
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Có lỗi đã xảy ra trong quá trình cập nhật dữ liệu. Xin vui lòng thử lại sau.");
            }

            TempData["SuccessMessage"] = "Cập nhật trạng thái thành công!";
            return RedirectToAction("Details", new { id });
        }


        // GET: PhieuXuat/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            // Validate ID
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Fetch PhieuXuat
            var phieuXuat = await _phieuXuatService.GetPhieuXuatByIdAsync(id);
            if (phieuXuat == null)
            {
                return NotFound();
            }

            // Begin database transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Delete associated ChiTietPhieuXuat
                var chiTietPhieuXuat = await _context.ChiTietPhieuXuats
                    .Where(c => c.MaPhieuXuat == id)
                    .ToListAsync();
                if (chiTietPhieuXuat.Any())
                {
                    _context.ChiTietPhieuXuats.RemoveRange(chiTietPhieuXuat);
                    await _context.SaveChangesAsync();
                }

                // Delete the PhieuXuat
                await _phieuXuatService.DeletePhieuXuatAsync(id);

                // Commit transaction if successful
                await transaction.CommitAsync();
            }
            // Handle concurrency errors
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
