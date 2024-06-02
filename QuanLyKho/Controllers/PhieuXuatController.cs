using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyKho.Data.Enums;
using QuanLyKho.Data.Interface;
using QuanLyKho.Data.Service;
using QuanLyKho.Helper;
using QuanLyKho.Models;
using QuanLyKho.ViewModels.PhieuNhapViewModels;
using QuanLyKho.ViewModels.PhieuXuatViewModels;
using System.Text;

namespace QuanLyKho.Controllers
{
    public class PhieuXuatController : Controller
    {
        private readonly QuanlykhoContext _context;
        private readonly IPhieuXuatService _phieuXuatService;
        public PhieuXuatController(QuanlykhoContext context, IPhieuXuatService phieuXuatService)
        {
            _context = context;
            _phieuXuatService = phieuXuatService;
        }

        public async Task<IActionResult> Index(string? searchString = null, string? trangThaiFilter = null)
        {
            var phieuXuatList = await _phieuXuatService.GetAllPhieuXuatAsync();
            var nguoiDungList = await _context.TaiKhoans.ToListAsync();

            var trangThaiList = phieuXuatList.Select(pn => pn.TrangThai).Distinct().Select(trangThai => new SelectListItem
            {
                Value = trangThai,
                Text = trangThai,
                Selected = trangThai == trangThaiFilter
            }).ToList();
            ViewBag.TrangThaiList = trangThaiList;

            searchString = string.IsNullOrEmpty(searchString) ? "" : searchString.ToLower();

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
                .Where(px =>
                    (string.IsNullOrEmpty(searchString) || px.MaPhieuXuat.ToLower().Contains(searchString)) &&
                    (trangThaiFilter == null || px.TrangThai == trangThaiFilter)
                    )
                    .ToList();
            return View(phieuXuatVM);
        }

        // GET: PhieuXuat/Create
        public async Task<IActionResult> Create(string maNguoiDung)
        {
            var nhanVienList = _context.TaiKhoans.Select(nv => new
            {
                MaNv = nv.MaNguoiDung,
                TenVn = nv.HoVaTen
            }).ToList();
            ViewBag.NhanVienList = nhanVienList.Select(nv => new SelectListItem
            {
                Value = nv.TenVn,
                Text = nv.TenVn 
            });
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

            var phieuXuatVM = new PhieuXuatCreateViewModel()
            {
                MaNguoiDung = maNguoiDung,
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
                    SoLuongXuat = 0
                }).ToList()
            };
            return View(phieuXuatVM);
        }

        // POST: PhieuXuat/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string maNguoiDung, PhieuXuatCreateViewModel phieuXuatCreateVM)
        {
            var nhanVienList = _context.TaiKhoans.Select(nv => new
            {
                MaNv = nv.MaNguoiDung,
                TenVn = nv.HoVaTen
            }).ToList();
            ViewBag.NhanVienList = nhanVienList.Select(nv => new SelectListItem
            {
                Value = nv.TenVn,
                Text = nv.TenVn
            });
            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    var phieuNhap = new PhieuXuatHang();
                    phieuNhap.MaPhieuXuat = AutoIncrementHelper.TaoMaPhieuXuatMoi(_context);
                    phieuNhap.MaNguoiDung = phieuXuatCreateVM.MaNguoiDung;
                    phieuNhap.NhanVienXuat = phieuXuatCreateVM.NguoiYeuCau?.Normalize(NormalizationForm.FormC);
                    phieuNhap.NgayXuat = DateTime.Now;
                    phieuNhap.TongTien = phieuXuatCreateVM.TongTien;
                    phieuNhap.GhiChu = phieuXuatCreateVM.GhiChu?.Normalize(NormalizationForm.FormC);
                    phieuNhap.TrangThai = EnumHelper.GetDisplayName(TrangThaiPhieuXuat.ChoDuyet).Normalize(NormalizationForm.FormC);

                    await _phieuXuatService.AddPhieuXuatAsync(phieuNhap);

                    foreach (var item in phieuXuatCreateVM.ThongTinThietBis)
                    {
                        if (item.SoLuongXuat > 0)
                        {
                            var parameters = new List<SqlParameter>();
                            parameters.Add(new SqlParameter("@maPhieuXuat", phieuNhap.MaPhieuXuat));
                            parameters.Add(new SqlParameter("@maThietBi", item.MaThietBi));
                            parameters.Add(new SqlParameter("@soLuongXuat", item.SoLuongXuat));
                            parameters.Add(new SqlParameter("@donGiaBan", item.GiaBan));
                            await _context.Database.ExecuteSqlRawAsync(
                                "EXEC ThemChiTietPhieuXuat @maPhieuXuat, @maThietBi, @soLuongXuat, @donGiaBan",
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
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong lúc tạo phiếu xuất.");
                }
            }
            return View(phieuXuatCreateVM);
        }

        // GET: PhieuXuat/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var phieuXuat = await _phieuXuatService.GetPhieuXuatByIdAsync(id);
            if (phieuXuat == null)
            {
                return NotFound();
            }

            var thongTinThietBiList = await _context.ThongTinThietBis.Include(tb => tb.MaNhaCungCapNavigation).ToListAsync();
            if (thongTinThietBiList == null)
            {
                return NotFound();
            }

            var chiTietPhieuXuatList = await _context.ChiTietPhieuXuats
                .FromSqlRaw("SELECT * FROM ChiTietPhieuXuat WHERE maPhieuXuat = {0}", id)
                .ToListAsync();

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
                    SoLuongXuat = ctpx.SoLuong,
                };
            }).ToList();

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

            phieuXuatDetailsVM.TrangThaiList = Enum.GetValues(typeof(TrangThaiPhieuXuat))
                .Cast<TrangThaiPhieuXuat>()
                .Select(value => new SelectListItem
                {
                    Value = EnumHelper.GetDisplayName(value),
                    Text = EnumHelper.GetDisplayName(value),
                }).ToList();

            // Set the text of selected value to the current status
            phieuXuatDetailsVM.SelectedTrangThai = phieuXuat.TrangThai;


            return View(phieuXuatDetailsVM);
        }

        // POST: PhieuXuat/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(string id, string SelectedTrangThai, PhieuXuatDetailsViewModel phieuXuatDetailsVM)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var phieuXuat = await _phieuXuatService.GetPhieuXuatByIdAsync(id);
            if (phieuXuat == null)
            {
                return NotFound();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                phieuXuat.TrangThai = SelectedTrangThai;
                if (SelectedTrangThai == "Đã duyệt")
                {
                    var chiTietPhieuXuatList = await _context.ChiTietPhieuXuats
                        .FromSqlRaw("SELECT * FROM ChiTietPhieuXuat WHERE maPhieuXuat = {0}", id)
                        .ToListAsync();

                    // Update SoLuongCon for each ThongTinThietBi
                    foreach (var chiTiet in chiTietPhieuXuatList)
                    {
                        var thietBi = await _context.ThongTinThietBis.FindAsync(chiTiet.MaThietBi);
                        if (thietBi != null && (thietBi.SoLuongCon - chiTiet.SoLuong >= 0))
                        {
                            thietBi.SoLuongCon -= chiTiet.SoLuong;
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

        // GET: PhieuXuat/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var phieuXuat = await _phieuXuatService.GetPhieuXuatByIdAsync(id);
            if (phieuXuat == null)
            {
                return NotFound();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var chiTietPhieuXuat = await _context.ChiTietPhieuXuats
                    .Where(c => c.MaPhieuXuat == id)
                    .ToListAsync();

                if (chiTietPhieuXuat.Any())
                {
                    _context.ChiTietPhieuXuats.RemoveRange(chiTietPhieuXuat);
                    await _context.SaveChangesAsync();
                }

                await _phieuXuatService.DeletePhieuXuatAsync(id);

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
