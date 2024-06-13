using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKho.Data.Interface;
using QuanLyKho.Helper;
using QuanLyKho.Models;
using QuanLyKho.ViewModels.NhanVienViewModels;
using QuanLyKho.ViewModels.OtherViewModels;

namespace QuanLyKho.Controllers
{
    [Authorize]
    public class NhanVienController : Controller
    {
        private readonly QuanlykhoContext _context;
        private readonly INhanVienService _nhanVienService;
        public NhanVienController(QuanlykhoContext context, INhanVienService nhanVienService)
        {
            _context = context;
            _nhanVienService = nhanVienService;
        }

        // GET: NhanVien/Index
        public async Task<IActionResult> Index(string searchString = "", string? vaiTroFilter = null, int pageNumber = 1, int pageSize = 10)
        {
            // Fetch the list of VaiTros asynchronously and populate ViewBag
            var vaiTroList = await _context.VaiTros.Select(lsh => new SelectListItem
            {
                Value = lsh.MaVaiTro.ToString(),
                Text = lsh.TenVaiTro,
                Selected = lsh.MaVaiTro == vaiTroFilter
            }).ToListAsync();
            ViewBag.VaiTroList = vaiTroList;

            // Normalize search string to lowercase
            searchString = string.IsNullOrEmpty(searchString) ? "" : searchString.ToLower();

            // Query to fetch NhanVienViewModels with optional filtering and searching
            var nhanVienQuery = from nv in _context.TaiKhoans
                                join vt in _context.VaiTros on nv.MaVaiTro equals vt.MaVaiTro into nvvt
                                from vt in nvvt.DefaultIfEmpty()
                                where (string.IsNullOrEmpty(searchString) || nv.HoVaTen.ToLower().Contains(searchString)) &&
                                      (string.IsNullOrEmpty(vaiTroFilter) || nv.MaVaiTro == vaiTroFilter)
                                select new NhanVienViewModel
                                {
                                    MaNguoiDung = nv.MaNguoiDung,
                                    HoVaTen = nv.HoVaTen,
                                    SoDienThoai = nv.SoDienThoai,
                                    Email = nv.Email,
                                    MaVaiTro = nv.MaVaiTro,
                                    TenVaiTro = vt.TenVaiTro ?? ""
                                };

            // Get total count of items before pagination
            int totalItems = await nhanVienQuery.CountAsync();

            // Apply pagination to the query
            var pagedNhanVienVM = await nhanVienQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Construct the view model for the Index view
            var nhanVienIndexVM = new NhanVienIndexViewModel
            {
                NhanViens = pagedNhanVienVM,
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = pageSize,
                    TotalItems = totalItems
                }
            };

            // Store search string and filter in ViewData to retain values in the view
            ViewData["searchString"] = searchString;
            ViewData["vaiTroFilter"] = vaiTroFilter;

            // Return the Index view with the constructed view model
            return View(nhanVienIndexVM);
        }


        // GET: NhanVien/Details/5
        public async Task<IActionResult> Details(string id)
        {
            // Query to fetch NhanVienViewModel by id
            var nhanVienVM = await _context.TaiKhoans
                .Where(nv => nv.MaNguoiDung == id)
                .Select(nv => new NhanVienViewModel
                {
                    MaNguoiDung = nv.MaNguoiDung,
                    HoVaTen = nv.HoVaTen,
                    SoDienThoai = nv.SoDienThoai,
                    Email = nv.Email,
                    MaVaiTro = nv.MaVaiTro,
                    TenVaiTro = nv.MaVaiTroNavigation.TenVaiTro // Navigation property used for VaiTro name
                })
                .FirstOrDefaultAsync();

            // Check if NhanVienViewModel is found
            if (nhanVienVM == null)
            {
                return NotFound(); // Return 404 Not Found if NhanVien is not found
            }

            // Return the Details view with the fetched NhanVienViewModel
            return View(nhanVienVM);
        }


        // GET: NhanVien/Create
        public async Task<IActionResult> Create()
        {
            // Fetch and prepare VaiTro list for the dropdown asynchronously
            var vaiTroList = await _context.VaiTros
                .Select(vt => new { MaVt = vt.MaVaiTro, TenVt = vt.TenVaiTro })
                .ToListAsync();

            // Assign the VaiTro list to ViewBag for use in the view
            ViewBag.VaiTroList = new SelectList(vaiTroList, "MaVt", "TenVt");

            // Return the Create view
            return View();
        }

        // POST: NhanVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhanVienCreateViewModel nhanVienCreateVM)
        {
            // Fetch and prepare VaiTro list for the dropdown asynchronously
            var vaiTroList = await _context.VaiTros
                .Select(vt => new { MaVt = vt.MaVaiTro, TenVt = vt.TenVaiTro })
                .ToListAsync();

            // Assign the VaiTro list to ViewBag for use in the view
            ViewBag.VaiTroList = new SelectList(vaiTroList, "MaVt", "TenVt");

            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Validate that the email does not already exist
                if (await _context.TaiKhoans.AnyAsync(tk => tk.Email == nhanVienCreateVM.Email))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                    return View(nhanVienCreateVM);
                }

                // Validate that the username does not already exist
                if (await _context.TaiKhoans.AnyAsync(tk => tk.TenDangNhap == nhanVienCreateVM.TenDangNhap))
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại");
                    return View(nhanVienCreateVM);
                }

                // Validate that the phone number does not already exist
                if (await _context.TaiKhoans.AnyAsync(tk => tk.SoDienThoai == nhanVienCreateVM.SoDienThoai))
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại đã tồn tại");
                    return View(nhanVienCreateVM);
                }

                // Create a new NhanVien entity
                var nhanVien = new TaiKhoan
                {
                    MaNguoiDung = AutoIncrementHelper.TaoMaNguoiDungMoi(_context), // Generate a new unique MaNguoiDung
                    TenDangNhap = nhanVienCreateVM.TenDangNhap,
                    MatKhau = nhanVienCreateVM.MatKhau,
                    Email = nhanVienCreateVM.Email,
                    SoDienThoai = nhanVienCreateVM.SoDienThoai,
                    HoVaTen = nhanVienCreateVM.HoVaTen,
                    MaVaiTro = nhanVienCreateVM.MaVaiTro,
                    TrangThai = "Active" // Set the initial status to Active
                };

                // Add the new NhanVien to the database
                await _nhanVienService.AddNhanvienAsync(nhanVien);

                // Redirect to the Index action after successful creation
                return RedirectToAction("Index");
            }

            // If model state is not valid, return the view with the provided data
            return View(nhanVienCreateVM);
        }

        // GET: NhanVien/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            // Fetch the NhanVien by ID asynchronously
            var nhanVien = await _nhanVienService.GetNhanvienByIdAsync(id);
            if (nhanVien == null)
            {
                // Return 404 Not Found if the NhanVien does not exist
                return NotFound();
            }

            // Fetch and prepare VaiTro list for the dropdown asynchronously
            var vaiTroList = await _context.VaiTros
                .Select(vt => new { MaVt = vt.MaVaiTro, TenVt = vt.TenVaiTro })
                .ToListAsync();

            // Assign the VaiTro list to ViewBag for use in the view
            ViewBag.VaiTroList = new SelectList(vaiTroList, "MaVt", "TenVt", nhanVien.MaVaiTro);

            // Create the ViewModel for editing
            var nhanVienEditVM = new NhanVienEditViewModel
            {
                MaNguoiDung = nhanVien.MaNguoiDung,
                TenDangNhap = nhanVien.TenDangNhap,
                Email = nhanVien.Email,
                SoDienThoai = nhanVien.SoDienThoai,
                HoVaTen = nhanVien.HoVaTen,
                MaVaiTro = nhanVien.MaVaiTro
            };

            // Return the Edit view with the ViewModel
            return View(nhanVienEditVM);
        }

        // POST: NhanVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, NhanVienEditViewModel nhanVienEditVM)
        {
            // Fetch and prepare VaiTro list for the dropdown asynchronously
            var vaiTroList = await _context.VaiTros
                .Select(vt => new { MaVt = vt.MaVaiTro, TenVt = vt.TenVaiTro })
                .ToListAsync();

            // Assign the VaiTro list to ViewBag for use in the view
            ViewBag.VaiTroList = new SelectList(vaiTroList, "MaVt", "TenVt", nhanVienEditVM.MaVaiTro);

            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Fetch the NhanVien by ID asynchronously
                var nhanVien = await _nhanVienService.GetNhanvienByIdAsync(id);
                if (nhanVien == null)
                {
                    // Return 404 Not Found if the NhanVien does not exist
                    return NotFound();
                }

                // Validate that the email does not already exist for another user
                if (await _context.TaiKhoans.AnyAsync(tk => tk.Email == nhanVienEditVM.Email && tk.MaNguoiDung != id))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                    return View(nhanVienEditVM);
                }

                // Validate that the username does not already exist for another user
                if (await _context.TaiKhoans.AnyAsync(tk => tk.TenDangNhap == nhanVienEditVM.TenDangNhap && tk.MaNguoiDung != id))
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại");
                    return View(nhanVienEditVM);
                }

                // Validate that the phone number does not already exist for another user
                if (await _context.TaiKhoans.AnyAsync(tk => tk.SoDienThoai == nhanVienEditVM.SoDienThoai && tk.MaNguoiDung != id))
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại đã tồn tại");
                    return View(nhanVienEditVM);
                }

                // Update the NhanVien with the new values
                nhanVien.TenDangNhap = nhanVienEditVM.TenDangNhap;
                nhanVien.Email = nhanVienEditVM.Email;
                nhanVien.SoDienThoai = nhanVienEditVM.SoDienThoai;
                nhanVien.HoVaTen = nhanVienEditVM.HoVaTen;
                nhanVien.MaVaiTro = nhanVienEditVM.MaVaiTro;

                // Save the changes asynchronously
                await _nhanVienService.UpdateNhanvienAsync(id, nhanVien);

                // Redirect to the Index action after successful update
                return RedirectToAction("Index");
            }

            // If model state is not valid, return the view with the provided data
            return View(nhanVienEditVM);
        }


        // GET: NhanVien/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            // Fetch the NhanVien by ID asynchronously
            var nhanVien = await _nhanVienService.GetNhanvienByIdAsync(id);
            if (nhanVien == null)
            {
                // Return 404 Not Found if the NhanVien does not exist
                return NotFound();
            }

            // Delete the NhanVien asynchronously
            await _nhanVienService.DeleteNhanvienAsync(id);

            // Redirect to the Index action after successful deletion
            return RedirectToAction("Index");
        }


        // GET: NhanVien/ChangePassword/5
        public async Task<IActionResult> ChangePassword(string id)
        {
            // Fetch the NhanVien by ID asynchronously
            var nhanVien = await _nhanVienService.GetNhanvienByIdAsync(id);
            if (nhanVien == null)
            {
                // Return 404 Not Found if the NhanVien does not exist
                return NotFound();
            }

            // Initialize the ChangePasswordViewModel with empty fields
            var changePasswordVM = new ChangePasswordViewModel
            {
                MatKhau = "",
                MatKhauMoi = "",
                XacNhanMatKhau = ""
            };

            // Return the ChangePassword view with the initialized ViewModel
            return View(changePasswordVM);
        }

        // POST: NhanVien/ChangePassword/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string id, ChangePasswordViewModel changePasswordVM)
        {
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Fetch the NhanVien by ID asynchronously
                var nhanVien = await _nhanVienService.GetNhanvienByIdAsync(id);
                if (nhanVien == null)
                {
                    // Return 404 Not Found if the NhanVien does not exist
                    return NotFound();
                }

                // Validate the current password
                if (changePasswordVM.MatKhau != nhanVien.MatKhau)
                {
                    ModelState.AddModelError("MatKhau", "Mật khẩu hiện tại không đúng");
                    return View(changePasswordVM);
                }

                // Validate the new password confirmation
                if (changePasswordVM.MatKhauMoi != changePasswordVM.XacNhanMatKhau)
                {
                    ModelState.AddModelError("XacNhanMatKhau", "Xác nhận mật khẩu mới không khớp");
                    return View(changePasswordVM);
                }

                // Update the password if all validations pass
                nhanVien.MatKhau = changePasswordVM.MatKhauMoi;
                await _nhanVienService.UpdateNhanvienAsync(id, nhanVien);

                // Set a success message to TempData
                TempData["SuccessChangePasswordMessage"] = "Mật khẩu đã được thay đổi thành công.";

                // Redirect to the Home Index action after successful password change
                return RedirectToAction("Index", "Home");
            }

            // If model state is not valid, return the view with the provided data
            return View(changePasswordVM);
        }


        // GET: NhanVien/ChangeProfile
        public async Task<IActionResult> ChangeProfile(string id)
        {
            // Fetch the NhanVien by ID asynchronously
            var nhanVien = await _nhanVienService.GetNhanvienByIdAsync(id);
            if (nhanVien == null)
            {
                // Return 404 Not Found if the NhanVien does not exist
                return NotFound();
            }

            // Fetch the corresponding VaiTro asynchronously
            var vaiTro = await _context.VaiTros.FirstOrDefaultAsync(vt => vt.MaVaiTro == nhanVien.MaVaiTro);

            // Initialize the ViewModel with NhanVien data
            var nhanVienVM = new NhanVienProfileViewModel
            {
                MaNguoiDung = nhanVien.MaNguoiDung,
                HoVaTen = nhanVien.HoVaTen,
                SoDienThoai = nhanVien.SoDienThoai,
                TenDangNhap = nhanVien.TenDangNhap,
                Email = nhanVien.Email,
                MaVaiTro = nhanVien.MaVaiTro,
                TenVaiTro = vaiTro?.TenVaiTro ?? ""
            };

            // Return the ChangeProfile view with the initialized ViewModel
            return View(nhanVienVM);
        }

        // POST: NhanVien/ChangeProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeProfile(string id, NhanVienProfileViewModel nhanVienProfileVM)
        {
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Fetch the NhanVien by ID asynchronously
                var nhanVien = await _nhanVienService.GetNhanvienByIdAsync(id);
                if (nhanVien == null)
                {
                    // Return 404 Not Found if the NhanVien does not exist
                    return NotFound();
                }

                // Validate that the phone number does not already exist for another user
                if (await _context.TaiKhoans.AnyAsync(tk => tk.SoDienThoai == nhanVienProfileVM.SoDienThoai && tk.MaNguoiDung != id))
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại đã tồn tại.");
                }
                else
                {
                    // Update the NhanVien with the new profile values
                    nhanVien.TenDangNhap = nhanVienProfileVM.TenDangNhap;
                    nhanVien.Email = nhanVienProfileVM.Email;
                    nhanVien.SoDienThoai = nhanVienProfileVM.SoDienThoai;
                    nhanVien.HoVaTen = nhanVienProfileVM.HoVaTen;

                    // Save the changes asynchronously
                    await _nhanVienService.UpdateNhanvienAsync(id, nhanVien);

                    // Set a success message to TempData
                    TempData["SuccessChangeProfileMessage"] = "Thông tin cá nhân đã được cập nhật thành công.";

                    // Redirect to the ChangeProfile action to display the updated profile
                    return RedirectToAction("ChangeProfile", new { id = id });
                }
            }

            // If model state is not valid, return the view with the provided data
            return View(nhanVienProfileVM);
        }

    }
}
