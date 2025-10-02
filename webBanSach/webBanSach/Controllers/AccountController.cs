using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webBanSach.Models;
using webBanSach.ViewModels;

namespace webBanSach.Controllers
{
    public class AccountController : Controller
    {
        private readonly WebBanSachContext _context;

        public AccountController(WebBanSachContext context)
        {
            _context = context;
        }

        // ===================== LOGIN =====================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _context.NguoiDungs
                .FirstOrDefault(u => u.Email == model.Email && u.LoaiNguoiDung == "User");

            if (user == null)
            {
                ViewBag.Error = "Email hoặc mật khẩu không đúng";
                return View(model);
            }

            // ✅ Kiểm tra trạng thái tài khoản
            if (user.TrangThai == "Khóa")
            {
                ViewBag.Error = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.";
                return View(model);
            }

            // ✅ Kiểm tra mật khẩu
            if (!BCrypt.Net.BCrypt.Verify(model.MatKhau, user.MatKhau))
            {
                ViewBag.Error = "Email hoặc mật khẩu không đúng";
                return View(model);
            }

            // ✅ Đăng nhập thành công
            HttpContext.Session.SetInt32("MaND", user.MaND);
            HttpContext.Session.SetString("UserName", user.HoTen ?? "User");
            HttpContext.Session.SetString("LoaiNguoiDung", user.LoaiNguoiDung);

            string avatarPath = string.IsNullOrEmpty(user.HinhAnh)
                ? "/images/nguoidung/default.jpg"
                : "/images/nguoidung/" + user.HinhAnh;

            HttpContext.Session.SetString("UserAvatar", avatarPath);

            return RedirectToAction("Index", "Home");
        }

        // ===================== REGISTER =====================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_context.NguoiDungs.Any(u => u.Email == model.Email))
            {
                ViewBag.Error = "Email đã tồn tại.";
                return View(model);
            }

            var user = new NguoiDung
            {
                HoTen = model.HoTen,
                Email = model.Email,
                MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau),
                SDT = model.SDT,
                DiaChi = model.DiaChi,
                LoaiNguoiDung = "User",
                TrangThai = "Hoạt động",
                NgayTao = DateTime.Now,
                HinhAnh = "default.jpg"
            };

            _context.Add(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Đăng ký thành công, vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        // ===================== LOGOUT =====================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ===================== PROFILE =====================
        [HttpGet]
        public IActionResult Profile()
        {
            int? maND = HttpContext.Session.GetInt32("MaND");
            if (maND == null) return RedirectToAction("Login");

            var user = _context.NguoiDungs.Find(maND);
            if (user == null) return RedirectToAction("Login");

            var vm = new ProfileVM
            {
                MaND = user.MaND,
                HoTen = user.HoTen,
                Email = user.Email,
                SDT = user.SDT,
                DiaChi = user.DiaChi,
                HinhAnh = user.HinhAnh
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(ProfileVM model, IFormFile? avatarFile)
        {
            int? maND = HttpContext.Session.GetInt32("MaND");
            if (maND == null) return RedirectToAction("Login");

            var user = _context.NguoiDungs.Find(maND);
            if (user == null) return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // ✅ Cập nhật thông tin
            user.HoTen = model.HoTen;
            user.SDT = model.SDT;
            user.DiaChi = model.DiaChi;

            // ✅ Đổi avatar nếu có upload
            if (avatarFile != null && avatarFile.Length > 0)
            {
                string fileName = $"nguoidung{user.MaND}_{Path.GetFileName(avatarFile.FileName)}";
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/nguoidung", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    avatarFile.CopyTo(stream);
                }
                user.HinhAnh = fileName;

                // Cập nhật Session Avatar
                HttpContext.Session.SetString("UserAvatar", "/images/nguoidung/" + fileName);
            }

            // ✅ Đổi mật khẩu nếu nhập
            if (!string.IsNullOrEmpty(model.MatKhauCu) && !string.IsNullOrEmpty(model.MatKhauMoi))
            {
                if (!BCrypt.Net.BCrypt.Verify(model.MatKhauCu, user.MatKhau))
                {
                    ViewBag.Error = "Mật khẩu cũ không đúng!";
                    return View(model);
                }

                user.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi);
            }

            _context.Update(user);
            _context.SaveChanges();

            // Cập nhật Session tên
            HttpContext.Session.SetString("UserName", user.HoTen);

            ViewBag.Success = "Cập nhật thông tin thành công!";
            return View(model);
        }

    }
}
