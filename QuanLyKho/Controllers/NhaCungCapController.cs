using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyKho.Data.Helper;
using QuanLyKho.Data.Interface;
using QuanLyKho.Models;
using QuanLyKho.ViewModels.NhaCungCapViewModel;

namespace QuanLyKho.Controllers
{
    [Authorize]
    public class NhaCungCapController : Controller
    {
        private readonly QuanlykhoContext _context;
        private readonly INhaCungCapService _nhaCungCapService;
        public NhaCungCapController(INhaCungCapService nhaCungCapService, QuanlykhoContext context)
        {
            _nhaCungCapService = nhaCungCapService;
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString = "")
        {
            searchString = string.IsNullOrEmpty(searchString) ? "" : searchString.ToLower();

            var listNhaCungCap = await _nhaCungCapService.GetAllNhaCungCapAsync();

            var nhaCungCapVM = listNhaCungCap
                .Select(ncc => new NhaCungCapViewModel
                {
                    MaNhaCungCap = ncc.MaNhaCungCap,
                    TenNhaCungCap = ncc.TenNhaCungCap,
                    DiaChi = ncc.DiaChi,
                    SoDienThoai = ncc.SoDienThoai,
                    Email = ncc.Email
                })
                .Where(ncc =>
                    string.IsNullOrEmpty(searchString) || ncc.TenNhaCungCap.ToLower().Contains(searchString)
                    )
                .ToList();

            ViewData["searchString"] = searchString;

            return View(nhaCungCapVM);
        }

        // GET: NhaCungCap/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var nhaCungCap = await _nhaCungCapService.GetNhaCungCapByIdAsync(id);
            if (nhaCungCap == null)
            {
                return NotFound();
            }

            var nhaCungCapVM = new NhaCungCapViewModel
            {
                MaNhaCungCap = nhaCungCap.MaNhaCungCap,
                TenNhaCungCap = nhaCungCap.TenNhaCungCap,
                DiaChi = nhaCungCap.DiaChi,
                SoDienThoai = nhaCungCap.SoDienThoai,
                Email = nhaCungCap.Email
            };

            return View(nhaCungCapVM);
        }

        // GET: NhaCungCap/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NhaCungCap/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhaCungCapViewModel nhaCungCapVM)
        {
            if (ModelState.IsValid)
            {
                var nhaCungCap = new NhaCungCap
                {
                    MaNhaCungCap= AutoIncrementHelper.TaoMaNhaCungCapMoi(_context),
                    TenNhaCungCap = nhaCungCapVM.TenNhaCungCap,
                    DiaChi = nhaCungCapVM.DiaChi,
                    SoDienThoai = nhaCungCapVM.SoDienThoai,
                    Email = nhaCungCapVM.Email
                };

                await _nhaCungCapService.AddNhaCungCapAsync(nhaCungCap);

                return RedirectToAction(nameof(Index));
            }
            return View(nhaCungCapVM);
        }

        // GET: NhaCungCap/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var nhaCungCap = await _nhaCungCapService.GetNhaCungCapByIdAsync(id);
            if (nhaCungCap == null)
            {
                return NotFound();
            }

            var nhaCungCapVM = new NhaCungCapViewModel
            {
                MaNhaCungCap = nhaCungCap.MaNhaCungCap,
                TenNhaCungCap = nhaCungCap.TenNhaCungCap,
                DiaChi = nhaCungCap.DiaChi,
                SoDienThoai = nhaCungCap.SoDienThoai,
                Email = nhaCungCap.Email
            };

            return View(nhaCungCapVM);
        }

        // POST: NhaCungCap/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, NhaCungCapViewModel nhaCungCapVM)
        {
            if (ModelState.IsValid)
            {
                var nhaCungCap = new NhaCungCap
                {
                    MaNhaCungCap = nhaCungCapVM.MaNhaCungCap,
                    TenNhaCungCap = nhaCungCapVM.TenNhaCungCap,
                    DiaChi = nhaCungCapVM.DiaChi,
                    SoDienThoai = nhaCungCapVM.SoDienThoai,
                    Email = nhaCungCapVM.Email
                };

                await _nhaCungCapService.UpdateNhaCungCapAsync(id, nhaCungCap);

                return RedirectToAction(nameof(Index));
            }
            return View(nhaCungCapVM);
        }

        // GET: NhaCungCap/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var nhaCungCap = await _nhaCungCapService.GetNhaCungCapByIdAsync(id);
            if (nhaCungCap == null)
            {
                return NotFound();
            }

            await _nhaCungCapService.DeleteNhaCungCapAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
