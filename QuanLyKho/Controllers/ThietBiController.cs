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
using System.Text;

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
        public async Task<IActionResult> Index(string searchString = "", string danhMucFilter = null, int pageNumber = 1, int pageSize = 10)
        {
            // Populate ViewBag with DanhMuc options for filtering
            var danhMucList = _context.DanhMucThietBis.Select(dm => new SelectListItem
            {
                Value = dm.MaDanhMuc,
                Text = dm.TenDanhMuc,
                Selected = dm.MaDanhMuc == danhMucFilter
            }).ToList();
            ViewBag.DanhMucList = danhMucList;

            // Normalize search string to lowercase
            searchString = string.IsNullOrEmpty(searchString) ? "" : searchString.ToLower();

            // Fetch all required data asynchronously
            var listThietBi = await _thietBiService.GetAllThietBiAsync();
            var listDanhMuc = await _context.DanhMucThietBis.ToListAsync();
            var listNhaCungCap = await _context.NhaCungCaps.ToListAsync();

            // Map ThietBi entities to ThietBiViewModels with filtering
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
                    (string.IsNullOrEmpty(searchString) || tb.TenThietBi.ToLower().Contains(searchString)) && // Filter by TenThietBi
                    (danhMucFilter == null || tb.MaDanhMuc == danhMucFilter) // Filter by MaDanhMuc
                )
                .ToList();

            // Apply pagination
            int totalItems = thietBiVM.Count;
            var pagedThietBiVM = thietBiVM.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Create the view model for the Index view
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

            // Retain search string and filter value after the page reloads
            ViewData["searchString"] = searchString;
            ViewData["danhMucFilter"] = danhMucFilter;

            // Return the Index view with the constructed view model
            return View(thietBiIndexVM);
        }

        // GET: ThietBi/Details/5
        public async Task<IActionResult> Details(string id)
        {
            // Fetch ThietBi entity by its ID asynchronously
            var thietBi = await _thietBiService.GetThietBiByIdAsync(id);

            // Check if ThietBi is found
            if (thietBi == null)
            {
                return NotFound(); // Return 404 Not Found if ThietBi is not found
            }

            // Fetch the related DanhMuc and NhaCungCap entities
            var danhMuc = await _context.DanhMucThietBis.FindAsync(thietBi.MaDanhMuc);
            var nhaCungCap = await _context.NhaCungCaps.FindAsync(thietBi.MaNhaCungCap);

            // Map ThietBi entity properties to a ThietBiViewModel
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

            // Return the Details view with the constructed ThietBiViewModel
            return View(thietBiVM);
        }


        // GET: ThietBi/Create
        public async Task<IActionResult> Create()
        {
            // Fetch data for danhMuc and nhaCungCap dropdowns
            var danhMucList = _context.DanhMucThietBis
                .Select(dm => new { MaDm = dm.MaDanhMuc, TenDm = dm.TenDanhMuc })
                .ToList();
            ViewBag.DanhMucList = new SelectList(danhMucList, "MaDm", "TenDm");

            var nhaCungCapList = _context.NhaCungCaps
                .Select(ncc => new { MaNcc = ncc.MaNhaCungCap, TenNcc = ncc.TenNhaCungCap })
                .ToList();
            ViewBag.NhaCungCapList = new SelectList(nhaCungCapList, "MaNcc", "TenNcc");

            // Return the view to create a new ThietBi
            return View();
        }

        // POST: ThietBi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThietBiCreateViewModel thietBiCreateVM)
        {
            // Fetch data for danhMuc and nhaCungCap dropdowns
            var danhMucList = _context.DanhMucThietBis
                .Select(dm => new { MaDm = dm.MaDanhMuc, TenDm = dm.TenDanhMuc })
                .ToList();
            ViewBag.DanhMucList = new SelectList(danhMucList, "MaDm", "TenDm");

            var nhaCungCapList = _context.NhaCungCaps
                .Select(ncc => new { MaNcc = ncc.MaNhaCungCap, TenNcc = ncc.TenNhaCungCap })
                .ToList();
            ViewBag.NhaCungCapList = new SelectList(nhaCungCapList, "MaNcc", "TenNcc");

            // Check if the model is valid
            if (ModelState.IsValid)
            {
                // Create a new ThongTinThietBi entity
                var thietBi = new ThongTinThietBi
                {
                    MaThietBi = AutoIncrementHelper.TaoMaThietBiMoi(_context), // Generate a unique ID
                    TenThietBi = thietBiCreateVM.TenThietBi,
                    MaDanhMuc = thietBiCreateVM.MaDanhMuc,
                    MaNhaCungCap = thietBiCreateVM.MaNhaCungCap,
                    ThoiGianBaoHanh = thietBiCreateVM.ThoiGianBaoHanh.ToString(),
                    Model = thietBiCreateVM.Model,
                    NhaSanXuat = thietBiCreateVM.NhaSanXuat,
                    MoTa = thietBiCreateVM.MoTa ?? "",
                    TrangThai = EnumHelper.GetDisplayName(TrangThaiThietBiEnum.HetHang),
                    SoLuongCon = 0,
                    GiaBan = thietBiCreateVM.GiaBan != null ? (decimal)thietBiCreateVM.GiaBan : 0
                };

                // Add the new ThietBi to the database
                await _thietBiService.AddThietBiAsync(thietBi);

                // Redirect to Index after successful creation
                return RedirectToAction(nameof(Index));
            }

            // Return the view with the model to display any validation errors
            return View(thietBiCreateVM);
        }


        // GET: ThietBi/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            // Fetch ThietBi entity by ID
            var thietBi = await _thietBiService.GetThietBiByIdAsync(id);
            if (thietBi == null)
            {
                return NotFound(); // Return 404 if ThietBi not found
            }

            // Prepare data for DanhMuc dropdown list
            var danhMucList = _context.DanhMucThietBis.Select(dm => new
            {
                MaDm = dm.MaDanhMuc,
                TenDm = dm.TenDanhMuc
            }).ToList();
            ViewBag.DanhMucList = new SelectList(danhMucList, "MaDm", "TenDm");

            // Prepare data for NhaCungCap dropdown list
            var nhaCungCapList = _context.NhaCungCaps.Select(ncc => new
            {
                MaNcc = ncc.MaNhaCungCap,
                TenNcc = ncc.TenNhaCungCap
            }).ToList();
            ViewBag.NhaCungCapList = new SelectList(nhaCungCapList, "MaNcc", "TenNcc");

            // Map ThietBi entity to ThietBiEditViewModel
            var thietBiEditVM = new ThietBiEditViewModel
            {
                MaThietBi = thietBi.MaThietBi,
                TenThietBi = thietBi.TenThietBi,
                MaDanhMuc = thietBi.MaDanhMuc,
                MaNhaCungCap = thietBi.MaNhaCungCap,
                ThoiGianBaoHanh = int.TryParse(thietBi.ThoiGianBaoHanh, out int thoiGianBaoHanh) ? thoiGianBaoHanh : 0, // Handle potential parsing errors
                Model = thietBi.Model,
                NhaSanXuat = thietBi.NhaSanXuat,
                MoTa = thietBi.MoTa,
                TrangThai = thietBi.TrangThai,
                SoLuongCon = thietBi.SoLuongCon,
                GiaBan = thietBi.GiaBan
            };

            // Return the Edit view with the mapped view model
            return View(thietBiEditVM);
        }

        // POST: ThietBi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ThietBiEditViewModel thietBiEditVM)
        {
            // Fetch data for danhMuc and nhaCungCap dropdowns
            var danhMucList = _context.DanhMucThietBis
                .Select(dm => new { MaDm = dm.MaDanhMuc, TenDm = dm.TenDanhMuc })
                .ToList();
            ViewBag.DanhMucList = new SelectList(danhMucList, "MaDm", "TenDm");

            var nhaCungCapList = _context.NhaCungCaps
                .Select(ncc => new { MaNcc = ncc.MaNhaCungCap, TenNcc = ncc.TenNhaCungCap })
                .ToList();
            ViewBag.NhaCungCapList = new SelectList(nhaCungCapList, "MaNcc", "TenNcc");

            // Check if IDs match
            if (id != thietBiEditVM.MaThietBi)
            {
                return NotFound();
            }

            // Validate model state
            if (ModelState.IsValid)
            {
                // Map updated view model back to a ThietBi entity
                var thietBi = new ThongTinThietBi
                {
                    MaThietBi = thietBiEditVM.MaThietBi,
                    TenThietBi = thietBiEditVM.TenThietBi,
                    MaDanhMuc = thietBiEditVM.MaDanhMuc,
                    MaNhaCungCap = thietBiEditVM.MaNhaCungCap,
                    ThoiGianBaoHanh = thietBiEditVM.ThoiGianBaoHanh.ToString(),
                    Model = thietBiEditVM.Model,
                    NhaSanXuat = thietBiEditVM.NhaSanXuat,
                    MoTa = thietBiEditVM.MoTa ?? "",
                    // Update TrangThai based on SoLuongCon
                    TrangThai = thietBiEditVM.SoLuongCon == 0
                        ? EnumHelper.GetDisplayName(TrangThaiThietBiEnum.HetHang)
                        : EnumHelper.GetDisplayName(TrangThaiThietBiEnum.ConHang),
                    SoLuongCon = thietBiEditVM.SoLuongCon,
                    GiaBan = thietBiEditVM.GiaBan
                };

                // Update the ThietBi in the database
                await _thietBiService.UpdateThietBiAsync(id, thietBi);

                // Redirect to Index on success
                return RedirectToAction(nameof(Index));
            }

            // If model state is invalid, return the view with the model
            return View(thietBiEditVM);
        }


        // GET: ThietBi/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            // Fetch ThietBi entity by ID
            var thietBi = await _thietBiService.GetThietBiByIdAsync(id);
            if (thietBi == null)
            {
                return NotFound(); // Return 404 if ThietBi not found
            }

            // Delete the ThietBi
            await _thietBiService.DeleteThietBiAsync(id);

            // Redirect to Index on success
            return RedirectToAction(nameof(Index));
        }

    }
}
