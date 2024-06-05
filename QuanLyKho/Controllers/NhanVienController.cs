using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKho.Data.Interface;
using QuanLyKho.Helper;
using QuanLyKho.Models;
using QuanLyKho.ViewModels.NhanVienViewModels;
using QuanLyKho.ViewModels.OtherViewModels;
using System.Globalization;
using System.Security.Claims;

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
        public async Task<IActionResult> Index(string searchString = "", string? vaiTroFilter = null, int pageNumber = 1, int pageSize = 10)
        {
            // ViewBag
            var vaiTroList = _context.VaiTros.Select(lsh => new SelectListItem
            {
                Value = lsh.MaVaiTro.ToString(),
                Text = lsh.TenVaiTro,
                Selected = lsh.MaVaiTro == vaiTroFilter
            }).ToList();
            ViewBag.VaiTroList = vaiTroList;

            searchString = string.IsNullOrEmpty(searchString) ? "" : searchString.ToLower();
            var listNhanVien = await _nhanVienService.GetAllNhanviensAsync();
            var listVaiTro = await _context.VaiTros.ToListAsync();

            var nhanVienVM = listNhanVien
                .Select(nv =>
                {
                    var vaiTro = listVaiTro.FirstOrDefault(vt => vt.MaVaiTro == nv.MaVaiTro);
                    return new NhanVienViewModel
                    {
                        MaNguoiDung = nv.MaNguoiDung,
                        HoVaTen = nv.HoVaTen,
                        SoDienThoai = nv.SoDienThoai,
                        Email = nv.Email,
                        MaVaiTro = nv.MaVaiTro,
                        TenVaiTro = vaiTro?.TenVaiTro ?? ""
                    };
                })
                .Where(nv =>
                    (string.IsNullOrEmpty(searchString) || nv.HoVaTen.ToLower().Contains(searchString)) && // Search by HoVaTen
                    (vaiTroFilter == null || nv.MaVaiTro == vaiTroFilter) // Filter by MaVaiTro
                    )
                    .ToList();

            // Pagination
            int totalItems = nhanVienVM.Count;
            var pagedNhanVienVM = nhanVienVM.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

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

            ViewData["searchString"] = searchString; // Keep search string after search
            ViewData["vaiTroFilter"] = vaiTroFilter; // Keep vaiTroFilter after filter

            return View(nhanVienIndexVM);
        }

        // GET: NhanVien/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var nhanVien = await _nhanVienService.GetNhanvienByIdAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            var vaiTro = await _context.VaiTros.FirstOrDefaultAsync(vt => vt.MaVaiTro == nhanVien.MaVaiTro);

            var nhanVienVM = new NhanVienViewModel
            {
                MaNguoiDung = nhanVien.MaNguoiDung,
                HoVaTen = nhanVien.HoVaTen,
                SoDienThoai = nhanVien.SoDienThoai,
                Email = nhanVien.Email,
                MaVaiTro = nhanVien.MaVaiTro,
                TenVaiTro = vaiTro?.TenVaiTro ?? ""
            };

            return View(nhanVienVM);
        }

        // GET: NhanVien/Create
        public async Task<IActionResult> Create()
        {
            var vaiTroList = _context.VaiTros.Select(vt => new
            {
                MaVt = vt.MaVaiTro,
                TenVt = vt.TenVaiTro
            }).ToList();
            ViewBag.VaiTroList = new SelectList(vaiTroList, "MaVt", "TenVt");
            return View();
        }

        // POST: NhanVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhanVienCreateViewModel nhanVienCreateVM)
        {
            var vaiTroList = _context.VaiTros.Select(vt => new
            {
                MaVt = vt.MaVaiTro,
                TenVt = vt.TenVaiTro
            }).ToList();
            ViewBag.VaiTroList = new SelectList(vaiTroList, "MaVt", "TenVt");

            if (ModelState.IsValid)
            {
                if (_context.TaiKhoans.Any(tk => tk.Email == nhanVienCreateVM.Email))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                    return View(nhanVienCreateVM);
                }
                if (_context.TaiKhoans.Any(tk => tk.TenDangNhap == nhanVienCreateVM.TenDangNhap))
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại");
                    return View(nhanVienCreateVM);
                }
                if (_context.TaiKhoans.Any(tk => tk.SoDienThoai == nhanVienCreateVM.SoDienThoai))
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại đã tồn tại");
                    return View(nhanVienCreateVM);
                }

                // Create new NhanVien
                var nhanVien = new TaiKhoan();
                nhanVien.MaNguoiDung = AutoIncrementHelper.TaoMaNguoiDungMoi(_context);
                nhanVien.TenDangNhap = nhanVienCreateVM.TenDangNhap;
                nhanVien.MatKhau = nhanVienCreateVM.MatKhau;
                nhanVien.Email = nhanVienCreateVM.Email;
                nhanVien.SoDienThoai = nhanVienCreateVM.SoDienThoai;
                nhanVien.HoVaTen = nhanVienCreateVM.HoVaTen;
                nhanVien.MaVaiTro = nhanVienCreateVM.MaVaiTro;
                nhanVien.TrangThai = "Active";

                await _nhanVienService.AddNhanvienAsync(nhanVien);
                return RedirectToAction("Index");
            }
            return View(nhanVienCreateVM);
        }

        // GET: NhanVien/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var nhanVien = await _nhanVienService.GetNhanvienByIdAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            var vaiTroList = _context.VaiTros.Select(vt => new
            {
                MaVt = vt.MaVaiTro,
                TenVt = vt.TenVaiTro
            }).ToList();
            ViewBag.VaiTroList = new SelectList(vaiTroList, "MaVt", "TenVt", nhanVien.MaVaiTro);

            var nhanVienEditVM = new NhanVienEditViewModel
            {
                MaNguoiDung = nhanVien.MaNguoiDung,
                TenDangNhap = nhanVien.TenDangNhap,
                Email = nhanVien.Email,
                SoDienThoai = nhanVien.SoDienThoai,
                HoVaTen = nhanVien.HoVaTen,
                MaVaiTro = nhanVien.MaVaiTro
            };

            return View(nhanVienEditVM);
        }

        // POST: NhanVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, NhanVienEditViewModel nhanVienEditVM)
        {
            var vaiTroList = _context.VaiTros.Select(vt => new
            {
                MaVt = vt.MaVaiTro,
                TenVt = vt.TenVaiTro
            }).ToList();
            ViewBag.VaiTroList = new SelectList(vaiTroList, "MaVt", "TenVt", nhanVienEditVM.MaVaiTro);

            if (ModelState.IsValid)
            {
                var nhanVien = await _nhanVienService.GetNhanvienByIdAsync(id);
                if (nhanVien == null)
                {
                    return NotFound();
                }

                if (_context.TaiKhoans.Any(tk => tk.Email == nhanVienEditVM.Email && tk.MaNguoiDung != id))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                    return View(nhanVienEditVM);
                }
                if (_context.TaiKhoans.Any(tk => tk.TenDangNhap == nhanVienEditVM.TenDangNhap && tk.MaNguoiDung != id))
                {
                    ModelState.AddModelError("Cccd", "Tên đăng nhập đã tồn tại");
                    return View(nhanVienEditVM);
                }
                if (_context.TaiKhoans.Any(tk => tk.SoDienThoai == nhanVienEditVM.SoDienThoai && tk.MaNguoiDung != id))
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại đã tồn tại");
                    return View(nhanVienEditVM);
                }
                else
                {
                    nhanVien.MaNguoiDung = id;
                    nhanVien.TenDangNhap = nhanVienEditVM.TenDangNhap;
                    nhanVien.Email = nhanVienEditVM.Email;
                    nhanVien.SoDienThoai = nhanVienEditVM.SoDienThoai;
                    nhanVien.HoVaTen = nhanVienEditVM.HoVaTen;
                    nhanVien.MaVaiTro = nhanVienEditVM.MaVaiTro;

                    await _nhanVienService.UpdateNhanvienAsync(id, nhanVien);
                    return RedirectToAction("Index");
                }
            }
            return View(nhanVienEditVM);
        }

        // GET: NhanVien/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var nhanVien = await _nhanVienService.GetNhanvienByIdAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            await _nhanVienService.DeleteNhanvienAsync(id);
            return RedirectToAction("Index");
        }

        // GET: NhanVien/ChangePassword/5
        public async Task<IActionResult> ChangePassword(string id)
        {
            var nhanVien = await _nhanVienService.GetNhanvienByIdAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            var changePasswordVM = new ChangePasswordViewModel
            {
                MatKhau = "",
                MatKhauMoi = "",
                XacNhanMatKhau = ""
            };

            return View(changePasswordVM);
        }

        // POST: NhanVien/ChangePassword/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string id, ChangePasswordViewModel changePasswordVM)
        {
            if (ModelState.IsValid)
            {
                var nhanVien = await _nhanVienService.GetNhanvienByIdAsync(id);
                if (nhanVien == null)
                {
                    return NotFound();
                }

                if (changePasswordVM.MatKhau != nhanVien.MatKhau)
                {
                    ModelState.AddModelError("MatKhau", "Mật khẩu không đúng");
                    return View(changePasswordVM);
                }
                if (changePasswordVM.MatKhauMoi != changePasswordVM.XacNhanMatKhau)
                {
                    ModelState.AddModelError("XacNhanMatKhau", "Xác nhận mật khẩu không đúng");
                    return View(changePasswordVM);
                }
                else
                {
                    nhanVien.MatKhau = changePasswordVM.MatKhauMoi;
                    await _nhanVienService.UpdateNhanvienAsync(id, nhanVien);
                    TempData["SuccessChangePasswordMessage"] = "Mật khẩu đã được thay đổi thành công.";
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(changePasswordVM);
        }

        // GET: NhanVien/Profile
        public async Task<IActionResult> ChangeProfile(string id)
        {
            var nhanVien = await _nhanVienService.GetNhanvienByIdAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            var vaiTro = await _context.VaiTros.FirstOrDefaultAsync(vt => vt.MaVaiTro == nhanVien.MaVaiTro);

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

            return View(nhanVienVM);
        }

        // POST: NhanVien/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeProfile(string id, NhanVienProfileViewModel nhanVienProfileVM)
        {
            if (ModelState.IsValid)
            {
                var nhanVien = await _nhanVienService.GetNhanvienByIdAsync(id);
                if (nhanVien == null)
                {
                    return NotFound();
                }

                if (_context.TaiKhoans.Any(tk => tk.SoDienThoai == nhanVienProfileVM.SoDienThoai && tk.MaNguoiDung != id))
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại đã tồn tại.");
                }
                else
                {
                    nhanVien.TenDangNhap = nhanVienProfileVM.TenDangNhap;
                    nhanVien.Email = nhanVienProfileVM.Email;
                    nhanVien.SoDienThoai = nhanVienProfileVM.SoDienThoai;
                    nhanVien.HoVaTen = nhanVienProfileVM.HoVaTen;

                    await _nhanVienService.UpdateNhanvienAsync(id, nhanVien);
                    TempData["SuccessChangeProfileMessage"] = "Thông tin cá nhân đã được cập nhật thành công.";
                    return RedirectToAction("ChangeProfile", new { id = id });
                }
            }
            return View(nhanVienProfileVM);
        }
    }
}
