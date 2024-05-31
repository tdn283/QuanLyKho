using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyKho.Data.Interface;
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
    }
}
