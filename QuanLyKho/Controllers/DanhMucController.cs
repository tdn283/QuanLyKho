using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyKho.Data.Interface;
using QuanLyKho.Data.Service;
using QuanLyKho.Helper;
using QuanLyKho.Models;
using QuanLyKho.ViewModels.DanhMucViewModels;
using System.Net.WebSockets;

namespace QuanLyKho.Controllers
{
    [Authorize]
    public class DanhMucController : Controller
    {
        private readonly QuanlykhoContext _context;
        private readonly IDanhMucService _danhMucService;
        public DanhMucController(QuanlykhoContext context, IDanhMucService danhMucService)
        {
            _context = context;
            _danhMucService = danhMucService;
        }
        public async Task<IActionResult> Index(string? danhMucFilter)
        {
            var danhMucList = _context.DanhMucThietBis.Select(dm => new SelectListItem
            {
                Value = dm.MaDanhMuc,
                Text = dm.TenDanhMuc,
                Selected = dm.MaDanhMuc == danhMucFilter
            }).ToList();

            ViewBag.DanhMucList = danhMucList;

            var listDanhMuc = await _danhMucService.GetAllDanhMucAsync();
            var danhMucVM = listDanhMuc
                .Select(dm =>
                {
                    return new DanhMucViewModel
                    {
                        MaDanhMuc = dm.MaDanhMuc,
                        TenDanhMuc = dm.TenDanhMuc,
                        MoTa = dm.MoTa ?? ""
                    };
                })
                .Where(dm =>
                (danhMucFilter == null || dm.MaDanhMuc == danhMucFilter)
                ).ToList();
            return View(danhMucVM);
        }

        // GET: DanhMuc/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DanhMuc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DanhMucCreateViewModel danhMucCreateVM)
        {
            if (ModelState.IsValid)
            {
                var danhMuc = new DanhMucThietBi
                {
                    MaDanhMuc = AutoIncrementHelper.TaoMaDanhMucMoi(_context),
                    TenDanhMuc = danhMucCreateVM.TenDanhMuc,
                    MoTa = danhMucCreateVM.MoTa ?? ""
                };
                await _danhMucService.AddDanhMucAsync(danhMuc);
                return RedirectToAction(nameof(Index));
            }
            return View(danhMucCreateVM);
        }

        // GET: DanhMuc/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var danhMuc = await _danhMucService.GetDanhMucIdAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }
            var danhMucEditVM = new DanhMucEditViewModel
            {
                MaDanhMuc = danhMuc.MaDanhMuc,
                TenDanhMuc = danhMuc.TenDanhMuc,
                MoTa = danhMuc.MoTa ?? ""
            };
            return View(danhMucEditVM);
        }

        // POST: DanhMuc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, DanhMucEditViewModel danhMucEditVM)
        {
            if (id != danhMucEditVM.MaDanhMuc)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var danhMuc = new DanhMucThietBi
                {
                    MaDanhMuc = danhMucEditVM.MaDanhMuc,
                    TenDanhMuc = danhMucEditVM.TenDanhMuc,
                    MoTa = danhMucEditVM.MoTa ?? ""
                };
                await _danhMucService.UpdateDanhMucAsync(id, danhMuc);
                return RedirectToAction(nameof(Index));
            }
            return View(danhMucEditVM);
        }

        // GET: DanhMuc/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var danhMuc = await _danhMucService.GetDanhMucIdAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }
            var danhMucDetailVM = new DanhMucViewModel
            {
                MaDanhMuc = danhMuc.MaDanhMuc,
                TenDanhMuc = danhMuc.TenDanhMuc,
                MoTa = danhMuc.MoTa ?? ""
            };
            return View(danhMucDetailVM);
        }

        // DELETE: DanhMuc/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var danhMuc = await _danhMucService.GetDanhMucIdAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }

            // Get list of ThietBi with MaDanhMuc = id then set maDanhMuc = null
            var thietBiList = _context.ThongTinThietBis.Where(tb => tb.MaDanhMuc == id).ToList();
            foreach (var thietBi in thietBiList)
            {
                thietBi.MaDanhMuc = null;
            }
            await _context.SaveChangesAsync();


            await _danhMucService.DeleteDanhMucAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
