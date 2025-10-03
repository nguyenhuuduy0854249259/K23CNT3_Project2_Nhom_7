using Microsoft.AspNetCore.Mvc;
using webBanSach.Models;

namespace webBanSach.Controllers
{
    public class DanhGiaController : Controller
    {
        private readonly WebBanSachContext _context;

        public DanhGiaController(WebBanSachContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Add(int MaSach, int Diem, string BinhLuan)
        {
            // Lấy user từ session
            var userId = HttpContext.Session.GetInt32("MaND");
            if (userId == null)
            {
                // Chưa login → redirect về login
                return RedirectToAction("Login", "Account");
            }

            if (Diem < 1 || Diem > 5) Diem = 5;
            BinhLuan = string.IsNullOrWhiteSpace(BinhLuan) ? "Không có nhận xét" : BinhLuan.Trim();

            // Kiểm tra user đã đánh giá chưa
            var existing = _context.DanhGias
                .FirstOrDefault(d => d.MaSach == MaSach && d.MaND == userId);

            if (existing != null)
            {
                existing.Diem = Diem;
                existing.BinhLuan = BinhLuan;
                existing.NgayDG = DateTime.Now;
            }
            else
            {
                var dg = new DanhGia
                {
                    MaSach = MaSach,
                    MaND = userId.Value,
                    Diem = Diem,
                    BinhLuan = BinhLuan,
                    NgayDG = DateTime.Now
                };
                _context.DanhGias.Add(dg);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Sach", new { id = MaSach });
        }
    }
}
