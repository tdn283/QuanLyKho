using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKho.Data.Interface;
using QuanLyKho.Helper;
using QuanLyKho.Models;
using QuanLyKho.ViewModels.NhaCungCapViewModel;
using QuanLyKho.ViewModels.NhanVienViewModels;
using QuanLyKho.ViewModels.OtherViewModels;

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

        // GET: NhaCungCap/Index
        public async Task<IActionResult> Index(string searchString = "", int pageNumber = 1, int pageSize = 10)
        {
            // Normalize search string (convert to lowercase for case-insensitive search)
            searchString = string.IsNullOrEmpty(searchString) ? "" : searchString.ToLower();

            // Fetch the list of NhaCungCaps asynchronously
            var listNhaCungCap = await _nhaCungCapService.GetAllNhaCungCapAsync();

            // Map NhaCungCap entities into NhaCungCapViewModels with optional filtering
            var nhaCungCapVM = listNhaCungCap
                .Select(ncc => new NhaCungCapViewModel
                {
                    MaNhaCungCap = ncc.MaNhaCungCap,
                    TenNhaCungCap = ncc.TenNhaCungCap,
                    DiaChi = ncc.DiaChi,
                    SoDienThoai = ncc.SoDienThoai,
                    Email = ncc.Email
                })
                .Where(ncc => string.IsNullOrEmpty(searchString) ||
                              ncc.TenNhaCungCap.ToLower().Contains(searchString))
                .ToList();

            // Get total count of items before pagination
            int totalItems = nhaCungCapVM.Count;

            // Apply pagination to the query (get items for current page)
            var pagedNhaCungCapVM = nhaCungCapVM
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Construct the view model for the Index view
            var nhaCungCapIndexVM = new NhaCungCapIndexViewModel
            {
                NhaCungCaps = pagedNhaCungCapVM,
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = pageSize,
                    TotalItems = totalItems
                }
            };

            // Store search string in ViewData to retain value in the view
            ViewData["searchString"] = searchString;

            // Return the Index view with the constructed view model
            return View(nhaCungCapIndexVM);
        }

        // GET: NhaCungCap/Details/5
        public async Task<IActionResult> Details(string id)
        {
            // Fetch NhaCungCap by its ID asynchronously
            var nhaCungCap = await _nhaCungCapService.GetNhaCungCapByIdAsync(id);

            // Check if NhaCungCap is found
            if (nhaCungCap == null)
            {
                return NotFound(); // Return 404 Not Found if NhaCungCap not found
            }

            // Map NhaCungCap properties to a NhaCungCapViewModel
            var nhaCungCapVM = new NhaCungCapViewModel
            {
                MaNhaCungCap = nhaCungCap.MaNhaCungCap,
                TenNhaCungCap = nhaCungCap.TenNhaCungCap,
                DiaChi = nhaCungCap.DiaChi,
                SoDienThoai = nhaCungCap.SoDienThoai,
                Email = nhaCungCap.Email
            };

            // Return the Details view with the mapped NhaCungCapViewModel
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
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Create a new NhaCungCap entity from the ViewModel
                var nhaCungCap = new NhaCungCap
                {
                    MaNhaCungCap = AutoIncrementHelper.TaoMaNhaCungCapMoi(_context), // Generate a new unique MaNhaCungCap
                    TenNhaCungCap = nhaCungCapVM.TenNhaCungCap,
                    DiaChi = nhaCungCapVM.DiaChi,
                    SoDienThoai = nhaCungCapVM.SoDienThoai,
                    Email = nhaCungCapVM.Email
                };

                // Add the new NhaCungCap to the database asynchronously
                await _nhaCungCapService.AddNhaCungCapAsync(nhaCungCap);

                // Redirect to the Index action after successful creation
                return RedirectToAction(nameof(Index));
            }

            // If model state is invalid (validation errors), return the view with the model data to display errors
            return View(nhaCungCapVM);
        }


        // GET: NhaCungCap/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            // Fetch NhaCungCap by its ID asynchronously
            var nhaCungCap = await _nhaCungCapService.GetNhaCungCapByIdAsync(id);

            // Check if NhaCungCap is found
            if (nhaCungCap == null)
            {
                return NotFound(); // Return 404 Not Found if NhaCungCap is not found
            }

            // Map NhaCungCap properties to a NhaCungCapViewModel for editing
            var nhaCungCapVM = new NhaCungCapViewModel
            {
                MaNhaCungCap = nhaCungCap.MaNhaCungCap,
                TenNhaCungCap = nhaCungCap.TenNhaCungCap,
                DiaChi = nhaCungCap.DiaChi,
                SoDienThoai = nhaCungCap.SoDienThoai,
                Email = nhaCungCap.Email
            };

            // Return the Edit view with the mapped NhaCungCapViewModel
            return View(nhaCungCapVM);
        }

        // POST: NhaCungCap/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, NhaCungCapViewModel nhaCungCapVM)
        {
            // Check if the model state is valid 
            if (ModelState.IsValid)
            {
                // Create an updated NhaCungCap entity from the ViewModel
                var nhaCungCap = new NhaCungCap
                {
                    MaNhaCungCap = id,
                    TenNhaCungCap = nhaCungCapVM.TenNhaCungCap,
                    DiaChi = nhaCungCapVM.DiaChi,
                    SoDienThoai = nhaCungCapVM.SoDienThoai,
                    Email = nhaCungCapVM.Email
                };

                // Update the NhaCungCap in the database asynchronously
                await _nhaCungCapService.UpdateNhaCungCapAsync(id, nhaCungCap);

                // Redirect to the Index action after successful update
                return RedirectToAction(nameof(Index));
            }

            // If model state is invalid (validation errors), return the view with the model data to display errors
            return View(nhaCungCapVM);
        }


        // GET: NhaCungCap/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            // Fetch NhaCungCap by its ID asynchronously
            var nhaCungCap = await _nhaCungCapService.GetNhaCungCapByIdAsync(id);

            // Check if NhaCungCap exists
            if (nhaCungCap == null)
            {
                return NotFound(); // Return 404 Not Found if NhaCungCap is not found
            }

            if (await _context.ThongTinThietBis.AnyAsync(tb => tb.MaNhaCungCap == id))
            {
                // Set a error message to TempData
                TempData["ErrorDeleteNhaCungCapMessage"] = "Không thể xóa nhà cung cấp.";
                return RedirectToAction(nameof(Index)); // Or return an error view
            }

            await _context.SaveChangesAsync(); // Save changes to disassociate ThietBis

            // Delete the NhaCungCap asynchronously
            await _nhaCungCapService.DeleteNhaCungCapAsync(id);

            // Redirect to the Index action after successful deletion
            return RedirectToAction(nameof(Index));
        }

    }
}
