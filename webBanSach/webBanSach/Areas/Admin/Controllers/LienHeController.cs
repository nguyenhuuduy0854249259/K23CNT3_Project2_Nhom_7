using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webBanSach.Models;

namespace webBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LienHeController : Controller
    {
        private readonly WebBanSachContext _context;

        public LienHeController(WebBanSachContext context)
        {
            _context = context;
        }

        // GET: Admin/LienHe
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.LienHes
                .Include(l => l.MaNDNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(l =>
                    l.HoTen.Contains(search) ||
                    l.Email.Contains(search) ||
                    (l.TieuDe != null && l.TieuDe.Contains(search)));
            }

            var list = await query
                .OrderByDescending(l => l.NgayGui)
                .ToListAsync();

            ViewBag.Search = search;
            return View(list);
        }

        // GET: Admin/LienHe/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var lh = await _context.LienHes
                .Include(l => l.MaNDNavigation)
                .FirstOrDefaultAsync(l => l.MaLH == id);

            if (lh == null) return NotFound();

            return View(lh);
        }

        // POST: Admin/LienHe/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lh = await _context.LienHes.FindAsync(id);
            if (lh != null)
            {
                _context.LienHes.Remove(lh);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "🗑️ Xóa liên hệ thành công.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/LienHe/Toggle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            var lh = await _context.LienHes.FindAsync(id);
            if (lh != null)
            {
                lh.TrangThai = (lh.TrangThai == "DaXuLy") ? "ChuaXuLy" : "DaXuLy";
                _context.Update(lh);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = lh.TrangThai == "DaXuLy"
                    ? "✅ Đánh dấu đã xử lý."
                    : "↩️ Đã chuyển lại thành chưa xử lý.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
