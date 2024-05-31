using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using QuanLyKho.Models;
using QuanLyKho.ViewModels.AccountViewModels;

namespace QuanLyKho.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanlykhoContext _context;
        public AccountController(QuanlykhoContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User;
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            var taiKhoan = await _context.TaiKhoans
                .Include(tk => tk.MaVaiTroNavigation)
                .FirstOrDefaultAsync(tk => tk.TenDangNhap == loginVM.TenDangNhap);
            //if (taiKhoan == null || !BCrypt.Net.BCrypt.Verify(loginVM.MatKhau, taiKhoan.MatKhau))
            //{
            //    ModelState.AddModelError("Email", "Email hoặc mật khẩu không đúng");
            //    return View();
            //}
            // check password no BCrypt
            if (taiKhoan == null || loginVM.MatKhau != taiKhoan.MatKhau)
            {
                ModelState.AddModelError("Email", "Email hoặc mật khẩu không đúng");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, taiKhoan.Email),
                new Claim(ClaimTypes.Name, taiKhoan.HoVaTen),
                new Claim("Id", taiKhoan.MaNguoiDung.ToString()),
            };

            var vaiTroId = taiKhoan.MaVaiTro;
            var vaiTro = taiKhoan.MaVaiTroNavigation.TenVaiTro;
            claims.Add(new Claim(ClaimTypes.Role, vaiTro));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = loginVM.RememberMe
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            return RedirectToAction("Index", "Home");
        }
    }
}
