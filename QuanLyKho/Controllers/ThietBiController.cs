using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKho.Data.Enums;
using QuanLyKho.Data.Interface;
using QuanLyKho.Helper;
using QuanLyKho.Models;
using QuanLyKho.ViewModels.OtherViewModels;
using QuanLyKho.ViewModels.PhieuNhapViewModels;
using QuanLyKho.ViewModels.ThietBiViewModels;

namespace QuanLyKho.Controllers
{
    [Authorize]
    public class ThietBiController : Controller
    {
        private readonly QuanlykhoContext _context;
        private readonly IThietBiService _thietBiService;
        public ThietBiController(QuanlykhoContext context, IThietBiService thietBiService)
        {
            _context = context;
            _thietBiService = thietBiService;
        }
        public async Task<IActionResult> Index(string searchString = null, string danhMucFilter = null, int pageNumber = 1, int pageSize = 10)
        {
            // Viewbag
            var danhMucList = _context.DanhMucThietBis.Select(dm => new SelectListItem
            {
                Value = dm.MaDanhMuc,
                Text = dm.TenDanhMuc,
                Selected = dm.MaDanhMuc == danhMucFilter
            }).ToList();
            ViewBag.DanhMucList = danhMucList;

            searchString = string.IsNullOrEmpty(searchString) ? "" : searchString.ToLower();

            var listThietBi = await _thietBiService.GetAllThietBiAsync();
            var listDanhMuc = await _context.DanhMucThietBis.ToListAsync();
            var listNhaCungCap = await _context.NhaCungCaps.ToListAsync();

            var thietBiVM = listThietBi
                .Select(tb =>
                {
                    var danhMuc = listDanhMuc.FirstOrDefault(dm => dm.MaDanhMuc == tb.MaDanhMuc);
                    var nhaCungCap = listNhaCungCap.FirstOrDefault(ncc => ncc.MaNhaCungCap == tb.MaNhaCungCap);
                    return new ThietBiViewModel
                    {
                        MaThietBi = tb.MaThietBi,
                        TenThietBi = tb.TenThietBi,
                        MaDanhMuc = tb.MaDanhMuc ?? "",
                        TenDanhMuc = danhMuc?.TenDanhMuc ?? "",
                        MaNhaCungCap = tb.MaNhaCungCap ?? "",
                        TenNhaCungCap = nhaCungCap?.TenNhaCungCap ?? "",
                        ThoiGianBaoHanh = int.TryParse(tb.ThoiGianBaoHanh, out int thoiGianBaoHanh) ? thoiGianBaoHanh : 0,
                        Model = tb.Model,
                        NhaSanXuat = tb.NhaSanXuat,
                        MoTa = tb.MoTa,
                        TrangThai = tb.SoLuongCon > 0 ? EnumHelper.GetDisplayName(TrangThaiThietBiEnum.ConHang) : EnumHelper.GetDisplayName(TrangThaiThietBiEnum.HetHang),
                        SoLuongCon = tb.SoLuongCon,
                        GiaBan = tb.GiaBan
                    };
                })
                .Where(tb =>
                    (string.IsNullOrEmpty(searchString) || tb.TenThietBi.ToLower().Contains(searchString)) && // Search by TenThietBi
                    (danhMucFilter == null || tb.MaDanhMuc == danhMucFilter) // Filter by MaDanhMuc
                    )
                .ToList();

            // Pagination
            int totalItems = thietBiVM.Count;
            var pagedThietBiVM = thietBiVM.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var thietBiIndexVM = new ThietBiIndexViewModel
            {
                ThietBis = pagedThietBiVM,
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = pageSize,
                    TotalItems = totalItems
                }
            };

            ViewData["searchString"] = searchString; // Keep the search string after reloading the page
            ViewData["danhMucFilter"] = danhMucFilter; // Keep the filter after reloading the page

            return View(thietBiIndexVM);
        }

        // GET: ThietBi/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var thietBi = await _thietBiService.GetThietBiByIdAsync(id);
            if (thietBi == null)
            {
                return NotFound();
            }

            var danhMuc = await _context.DanhMucThietBis.FindAsync(thietBi.MaDanhMuc);
            var nhaCungCap = await _context.NhaCungCaps.FindAsync(thietBi.MaNhaCungCap);

            var thietBiVM = new ThietBiViewModel
            {
                MaThietBi = thietBi.MaThietBi,
                TenThietBi = thietBi.TenThietBi,
                MaDanhMuc = thietBi.MaDanhMuc,
                TenDanhMuc = danhMuc?.TenDanhMuc ?? "",
                MaNhaCungCap = thietBi.MaNhaCungCap,
                TenNhaCungCap = nhaCungCap?.TenNhaCungCap ?? "",
                ThoiGianBaoHanh = int.TryParse(thietBi.ThoiGianBaoHanh, out int thoiGianBaoHanh) ? thoiGianBaoHanh : 0,
                Model = thietBi.Model,
                NhaSanXuat = thietBi.NhaSanXuat,
                MoTa = thietBi.MoTa,
                TrangThai = thietBi.TrangThai,
                SoLuongCon = thietBi.SoLuongCon,
                GiaBan = thietBi.GiaBan
            };
            return View(thietBiVM);
        }

        // GET: ThietBi/Create
        public async Task<IActionResult> Create()
        {
            var danhMucList = _context.DanhMucThietBis.Select(dm => new
            {
                MaDm = dm.MaDanhMuc,
                TenDm = dm.TenDanhMuc
            }).ToList();
            ViewBag.DanhMucList = new SelectList(danhMucList, "MaDm", "TenDm");

            var nhaCungCapList = _context.NhaCungCaps.Select(ncc => new
            {
                MaNcc = ncc.MaNhaCungCap,
                TenNcc = ncc.TenNhaCungCap
            }).ToList();
            ViewBag.NhaCungCapList = new SelectList(nhaCungCapList, "MaNcc", "TenNcc");

            return View();
        }

        // POST: ThietBi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThietBiCreateViewModel thietBiCreateVM)
        {
            var danhMucList = _context.DanhMucThietBis.Select(dm => new
            {
                MaDm = dm.MaDanhMuc,
                TenDm = dm.TenDanhMuc
            }).ToList();
            ViewBag.DanhMucList = new SelectList(danhMucList, "MaDm", "TenDm");

            var nhaCungCapList = _context.NhaCungCaps.Select(ncc => new
            {
                MaNcc = ncc.MaNhaCungCap,
                TenNcc = ncc.TenNhaCungCap
            }).ToList();
            ViewBag.NhaCungCapList = new SelectList(nhaCungCapList, "MaNcc", "TenNcc");

            if (ModelState.IsValid)
            {
                var thietBi = new ThongTinThietBi
                {
                    MaThietBi = AutoIncrementHelper.TaoMaThietBiMoi(_context),
                    TenThietBi = thietBiCreateVM.TenThietBi,
                    MaDanhMuc = thietBiCreateVM.MaDanhMuc,
                    MaNhaCungCap = thietBiCreateVM.MaNhaCungCap,
                    ThoiGianBaoHanh = thietBiCreateVM.ThoiGianBaoHanh.ToString(),
                    Model = thietBiCreateVM.Model,
                    NhaSanXuat = thietBiCreateVM.NhaSanXuat,
                    MoTa = thietBiCreateVM.MoTa != null ? thietBiCreateVM.MoTa : "",
                    TrangThai = EnumHelper.GetDisplayName(TrangThaiThietBiEnum.HetHang),
                    SoLuongCon = 0,
                    GiaBan = thietBiCreateVM.GiaBan != null ? (decimal)thietBiCreateVM.GiaBan : 0
                };
                await _thietBiService.AddThietBiAsync(thietBi);
                return RedirectToAction(nameof(Index));
            }
            return View(thietBiCreateVM);
        }

        // GET: ThietBi/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var thietBi = await _thietBiService.GetThietBiByIdAsync(id);
            if (thietBi == null)
            {
                return NotFound();
            }

            var danhMucList = _context.DanhMucThietBis.Select(dm => new
            {
                MaDm = dm.MaDanhMuc,
                TenDm = dm.TenDanhMuc
            }).ToList();
            ViewBag.DanhMucList = new SelectList(danhMucList, "MaDm", "TenDm");

            var nhaCungCapList = _context.NhaCungCaps.Select(ncc => new
            {
                MaNcc = ncc.MaNhaCungCap,
                TenNcc = ncc.TenNhaCungCap
            }).ToList();
            ViewBag.NhaCungCapList = new SelectList(nhaCungCapList, "MaNcc", "TenNcc");

            var thietBiEditVM = new ThietBiEditViewModel
            {
                MaThietBi = thietBi.MaThietBi,
                TenThietBi = thietBi.TenThietBi,
                MaDanhMuc = thietBi.MaDanhMuc,
                MaNhaCungCap = thietBi.MaNhaCungCap,
                ThoiGianBaoHanh = int.TryParse(thietBi.ThoiGianBaoHanh, out int thoiGianBaoHanh) ? thoiGianBaoHanh : 0,
                Model = thietBi.Model,
                NhaSanXuat = thietBi.NhaSanXuat,
                MoTa = thietBi.MoTa,
                TrangThai = thietBi.TrangThai,
                SoLuongCon = thietBi.SoLuongCon,
                GiaBan = thietBi.GiaBan
            };
            return View(thietBiEditVM);
        }

        // POST: ThietBi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ThietBiEditViewModel thietBiEditVM)
        {
            var danhMucList = _context.DanhMucThietBis.Select(dm => new
            {
                MaDm = dm.MaDanhMuc,
                TenDm = dm.TenDanhMuc
            }).ToList();
            ViewBag.DanhMucList = new SelectList(danhMucList, "MaDm", "TenDm");

            var nhaCungCapList = _context.NhaCungCaps.Select(ncc => new
            {
                MaNcc = ncc.MaNhaCungCap,
                TenNcc = ncc.TenNhaCungCap
            }).ToList();
            ViewBag.NhaCungCapList = new SelectList(nhaCungCapList, "MaNcc", "TenNcc");

            if (id != thietBiEditVM.MaThietBi)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var thietBi = new ThongTinThietBi
                {
                    MaThietBi = thietBiEditVM.MaThietBi,
                    TenThietBi = thietBiEditVM.TenThietBi,
                    MaDanhMuc = thietBiEditVM.MaDanhMuc,
                    MaNhaCungCap = thietBiEditVM.MaNhaCungCap,
                    ThoiGianBaoHanh = thietBiEditVM.ThoiGianBaoHanh.ToString(),
                    Model = thietBiEditVM.Model,
                    NhaSanXuat = thietBiEditVM.NhaSanXuat,
                    MoTa = thietBiEditVM.MoTa != null ? thietBiEditVM.MoTa : "",
                    TrangThai = thietBiEditVM.SoLuongCon == 0 ? EnumHelper.GetDisplayName(TrangThaiThietBiEnum.HetHang) : EnumHelper.GetDisplayName(TrangThaiThietBiEnum.ConHang),
                    SoLuongCon = thietBiEditVM.SoLuongCon,
                    GiaBan = thietBiEditVM.GiaBan
                };
                await _thietBiService.UpdateThietBiAsync(id, thietBi);
                return RedirectToAction(nameof(Index));
            }
            return View(thietBiEditVM);
        }

        // GET: ThietBi/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var thietBi = await _thietBiService.GetThietBiByIdAsync(id);
            if (thietBi == null)
            {
                return NotFound();
            }

            await _thietBiService.DeleteThietBiAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
