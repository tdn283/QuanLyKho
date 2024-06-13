using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKho.Data.Interface;
using QuanLyKho.Data.Service;
using QuanLyKho.Helper;
using QuanLyKho.Models;
using QuanLyKho.ViewModels.DanhMucViewModels;
using QuanLyKho.ViewModels.OtherViewModels;
using QuanLyKho.ViewModels.PhieuNhapViewModels;
using System.Net.WebSockets;
using System.Text;

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
        // GET: DanhMuc/Index
        public async Task<IActionResult> Index(string searchString = "", int pageNumber = 1, int pageSize = 10)
        {
            // Normalize search string to lowercase
            searchString = string.IsNullOrEmpty(searchString) ? "" : searchString.ToLower();

            // Fetch all DanhMuc entities asynchronously
            var listDanhMuc = await _danhMucService.GetAllDanhMucAsync();

            // Map DanhMuc entities to DanhMucViewModels with filtering based on searchString
            var danhMucVM = listDanhMuc
                .Select(dm => new DanhMucViewModel
                {
                    MaDanhMuc = dm.MaDanhMuc,
                    TenDanhMuc = dm.TenDanhMuc,
                    MoTa = dm.MoTa ?? ""
                })
                .Where(dm => string.IsNullOrEmpty(searchString) ||
                              dm.TenDanhMuc.ToLower().Contains(searchString)) // Filter by TenDanhMuc
                .ToList();

            // Apply pagination
            int totalItems = danhMucVM.Count;
            var pagedDanhMucVM = danhMucVM.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Create the view model for the Index view
            var danhMucIndexVM = new DanhMucIndexViewModel
            {
                DanhMucs = pagedDanhMucVM,
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = pageSize,
                    TotalItems = totalItems
                }
            };

            // Retain search string after the page reloads
            ViewData["searchString"] = searchString;

            // Return the Index view with the constructed view model
            return View(danhMucIndexVM);
        }

        // GET: DanhMuc/Details/5
        public async Task<IActionResult> Details(string id)
        {
            // Fetch DanhMuc by its ID asynchronously
            var danhMuc = await _danhMucService.GetDanhMucIdAsync(id);

            // Check if DanhMuc is found
            if (danhMuc == null)
            {
                return NotFound(); // Return 404 Not Found if DanhMuc is not found
            }

            // Map DanhMuc properties to a DanhMucViewModel 
            var danhMucDetailVM = new DanhMucViewModel
            {
                MaDanhMuc = danhMuc.MaDanhMuc,
                TenDanhMuc = danhMuc.TenDanhMuc,
                MoTa = danhMuc.MoTa ?? "" // Handle null descriptions
            };

            // Return the Details view with the mapped DanhMucViewModel
            return View(danhMucDetailVM);
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
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Create a new DanhMucThietBi entity
                var danhMuc = new DanhMucThietBi
                {
                    MaDanhMuc = AutoIncrementHelper.TaoMaDanhMucMoi(_context), // Generate a unique ID
                    TenDanhMuc = danhMucCreateVM.TenDanhMuc,
                    MoTa = danhMucCreateVM.MoTa ?? ""
                };

                // Add the new DanhMucThietBi to the database asynchronously
                await _danhMucService.AddDanhMucAsync(danhMuc);

                // Redirect to the Index action after successful creation
                return RedirectToAction(nameof(Index));
            }

            // If model state is invalid (validation errors), return the view with the model data to display errors
            return View(danhMucCreateVM);
        }


        // GET: DanhMuc/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            // Fetch DanhMuc by its ID asynchronously
            var danhMuc = await _danhMucService.GetDanhMucIdAsync(id);

            // Check if DanhMuc is found
            if (danhMuc == null)
            {
                return NotFound(); // Return 404 Not Found if DanhMuc is not found
            }

            // Map DanhMuc properties to a DanhMucEditViewModel for editing
            var danhMucEditVM = new DanhMucEditViewModel
            {
                MaDanhMuc = danhMuc.MaDanhMuc,
                TenDanhMuc = danhMuc.TenDanhMuc,
                MoTa = danhMuc.MoTa ?? ""
            };

            // Return the Edit view with the mapped DanhMucEditViewModel
            return View(danhMucEditVM);
        }

        // POST: DanhMuc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, DanhMucEditViewModel danhMucEditVM)
        {
            // Ensure the ID in the URL matches the ID in the model
            if (id != danhMucEditVM.MaDanhMuc)
            {
                return NotFound();
            }

            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Create an updated DanhMucThietBi entity from the ViewModel
                var danhMuc = new DanhMucThietBi
                {
                    MaDanhMuc = danhMucEditVM.MaDanhMuc,
                    TenDanhMuc = danhMucEditVM.TenDanhMuc,
                    MoTa = danhMucEditVM.MoTa ?? ""
                };

                // Update the DanhMucThietBi in the database asynchronously
                await _danhMucService.UpdateDanhMucAsync(id, danhMuc);

                // Redirect to the Index action after successful update
                return RedirectToAction(nameof(Index));
            }

            // If model state is invalid, return the view with the model data to display errors
            return View(danhMucEditVM);
        }



        // GET: DanhMuc/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            // Fetch DanhMuc by its ID asynchronously
            var danhMuc = await _danhMucService.GetDanhMucIdAsync(id);

            // Check if DanhMuc is found
            if (danhMuc == null)
            {
                return NotFound(); // Return 404 Not Found if DanhMuc is not found
            }

            // Handle related ThietBis
            if (await _context.ThongTinThietBis.AnyAsync(tb => tb.MaDanhMuc == id))
            {
                // Set a error message to TempData
                TempData["ErrorDeleteDanhMucMessage"] = "Không thể xóa danh mục.";
                return RedirectToAction(nameof(Index)); // Or return an error view
            }

            // Delete the DanhMuc asynchronously
            await _danhMucService.DeleteDanhMucAsync(id);

            // Redirect to the Index action after successful deletion
            return RedirectToAction(nameof(Index));
        }

    }
}
