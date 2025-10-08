using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using webBanSach.Models;
using System.Linq;

namespace webBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminAccountController : Controller
    {
        private readonly WebBanSachContext _context;

        public AdminAccountController(WebBanSachContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminAccount/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("AdminId") != null)
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            return View();
        }

        // POST: Admin/AdminAccount/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(matKhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            var admin = _context.NguoiDungs
                .FirstOrDefault(a => a.Email == email
                                  && a.LoaiNguoiDung == "Admin"
                                  && a.TrangThai == "Hoạt động");

            if (admin != null && BCrypt.Net.BCrypt.Verify(matKhau, admin.MatKhau))
            {
                // Lưu session
                HttpContext.Session.SetInt32("AdminId", admin.MaND);
                HttpContext.Session.SetString("AdminName", admin.HoTen ?? "Admin");
                string avatarPath = string.IsNullOrEmpty(admin.HinhAnh)
                    ? "/images/admins/default.png"
                    : $"/images/admins/{admin.HinhAnh}";
                HttpContext.Session.SetString("AdminAvatar", avatarPath);

                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            ViewBag.Error = "Email hoặc mật khẩu không đúng";
            return View();
        }

        // GET: Admin/AdminAccount/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "AdminAccount", new { area = "Admin" });
        }
    }
}
