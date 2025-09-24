using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using webBanSach.Models;

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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string matKhau)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matKhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            // Lấy admin theo email, loại người dùng và trạng thái
            var admin = _context.NguoiDungs
                .FirstOrDefault(a => a.Email == email
                                  && a.LoaiNguoiDung == "Admin"
                                  && a.TrangThai == "Hoạt động");
          
            if (admin != null)
            {
                // Kiểm tra mật khẩu với BCrypt
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(matKhau, admin.MatKhau);
                if (isPasswordValid)
                {
                    // Lưu session
                    HttpContext.Session.SetInt32("MaND", admin.MaND);
                    HttpContext.Session.SetString("AdminName", admin.HoTen ?? "Admin");

                    // Lưu avatar từ wwwroot/images/Admin/
                    string avatarPath = string.IsNullOrEmpty(admin.HinhAnh)
                        ? "/images/admins/default.png" // ảnh mặc định nếu chưa có
                        : $"/images/admins/{admin.HinhAnh}";
                    HttpContext.Session.SetString("AdminAvatar", avatarPath);

                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
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
