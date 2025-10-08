using Microsoft.AspNetCore.Mvc;
using webBanSach.Models; // namespace model
using System;
using System.Threading.Tasks;

namespace webBanSach.Controllers
{
    public class LienHeController : Controller
    {
        private readonly WebBanSachContext _context;

        public LienHeController(WebBanSachContext context)
        {
            _context = context;
        }

        // Hiển thị form liên hệ
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Nhận dữ liệu người dùng gửi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LienHe model)
        {
            if (ModelState.IsValid)
            {
                model.NgayGui = DateTime.Now;
                model.TrangThai = "Chưa xử lý";

                _context.Add(model);
                await _context.SaveChangesAsync();

                ViewBag.ThongBao = "Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi sớm nhất có thể.";
                ModelState.Clear(); // Xóa dữ liệu sau khi gửi
                return View();
            }

            // Nếu có lỗi nhập liệu
            ViewBag.ThongBao = "Vui lòng nhập đầy đủ thông tin hợp lệ!";
            return View(model);
        }
    }
}