using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using QuanLyKho.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using QuanLyKho.ViewModels.PhieuNhapViewModels;
using QuanLyKho.ViewModels.PhieuXuatViewModels;
using QuanLyKho.ViewModels.OtherViewModels;

namespace QuanLyKho.Controllers
{
    public class HomeController : Controller
    {
        private readonly QuanlykhoContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, QuanlykhoContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var soNhanVien = _context.TaiKhoans.Count();
            var soThietBi = _context.ThongTinThietBis.Count();

            var phieuNhaps = _context.PhieuNhapHangs
                .Include(pn => pn.MaNguoiDungNavigation)
                .Where(pn => pn.TrangThai == "Đã nhập kho")
                .Select(pn => new PhieuNhapViewModel
                {
                    MaPhieuNhap = pn.MaPhieuNhap,
                    HoVaTen = pn.MaNguoiDungNavigation.HoVaTen,
                    NgayNhap = pn.NgayNhap.ToString("dd/MM/yyyy"),
                    TongTien = pn.TongTien,
                    TrangThai = pn.TrangThai,
                    GhiChu = pn.GhiChu
                })
                .ToList();

            var phieuXuats = _context.PhieuXuatHangs
                .Include(px => px.MaNguoiDungNavigation)
                .Where(px => px.TrangThai == "Đã duyệt")
                .Select(px => new PhieuXuatViewModel
                {
                    MaPhieuXuat = px.MaPhieuXuat,
                    HoVaTen = px.MaNguoiDungNavigation.HoVaTen,
                    NgayXuat = px.NgayXuat.ToString("dd/MM/yyyy"),
                    TongTien = px.TongTien,
                    TrangThai = px.TrangThai,
                    GhiChu = px.GhiChu
                })
                .ToList();

            // Parse dates and compute monthly sums
            var monthlyIncomeSums = phieuNhaps
                .Select(pn => new
                {
                    Date = DateTime.ParseExact(pn.NgayNhap, "dd/MM/yyyy", null),
                    pn.TongTien
                })
                .GroupBy(pn => new {pn.Date.Month, pn.Date.Year })
                .Select(g => new MonthlySumViewModel
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Total = g.Sum(pn => pn.TongTien)
                })
                .ToList();

            var monthlyExpenseSums = phieuXuats
                .Select(px => new
                {
                    Date = DateTime.ParseExact(px.NgayXuat, "dd/MM/yyyy", null),
                    px.TongTien
                })
                .GroupBy(px => new {px.Date.Month, px.Date.Year })
                .Select(g => new MonthlySumViewModel
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Total = g.Sum(px => px.TongTien)
                })
                .ToList();

            // Get the current date and calculate the start of the 12-month period
            var currentDate = DateTime.Now;
            var startDate = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(-11);
            var allMonths = new List<DateTime>();

            // Generate a list of all months within the past 12 months
            for (var i = 0; i < 12; i++)
            {
                allMonths.Add(startDate.AddMonths(i));
            }

            // Prepare dictionaries to efficiently look up income/expense data
            var incomeData = monthlyIncomeSums.ToDictionary(pn => new DateTime(pn.Year, pn.Month, 1), pn => pn.Total);
            var expenseData = monthlyExpenseSums.ToDictionary(px => new DateTime(px.Year, px.Month, 1), px => px.Total);

            var chartViewModel = new ChartViewModel
            {
                Labels = allMonths.Select(date => date.ToString("MM/yyyy")).ToList(),
                IncomeData = allMonths.Select(date => incomeData.GetValueOrDefault(date, 0)).ToList(),
                ExpenseData = allMonths.Select(date => expenseData.GetValueOrDefault(date, 0)).ToList()
            };

            // Calculate all-time total income and expenses
            decimal totalIncome = _context.PhieuXuatHangs.Where(px => px.TrangThai == "Đã duyệt").Sum(pn => pn.TongTien);
            decimal totalExpenses = _context.PhieuNhapHangs.Where(pn => pn.TrangThai == "Đã nhập kho").Sum(px => px.TongTien);

            // Calculate revenue and percentage
            decimal revenue = totalIncome - totalExpenses;
            decimal revenuePercentage = totalIncome > 0 ? (revenue / totalIncome) * 100 : 0;

            return View(Tuple.Create(soNhanVien, soThietBi, phieuNhaps, phieuXuats, chartViewModel, revenuePercentage));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
