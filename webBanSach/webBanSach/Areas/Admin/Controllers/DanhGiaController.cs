using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webBanSach.Models;

namespace webBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DanhGiaController : Controller
    {
        private readonly WebBanSachContext _context;
        public DanhGiaController(WebBanSachContext context)
        {
            _context = context;
        }

        // GET: Admin/DanhGia
        public async Task<IActionResult> Index(string? search)
        {
            var q = _context.DanhGias
                .Include(d => d.MaNDNavigation)
                .Include(d => d.MaSachNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                q = q.Where(d => d.MaNDNavigation.HoTen.Contains(search)
                              || d.MaSachNavigation.TenSach.Contains(search));
            }

            var list = await q.OrderByDescending(d => d.NgayDG).ToListAsync();
            return View(list);
        }

        // POST: Admin/DanhGia/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dg = await _context.DanhGias
                .FirstOrDefaultAsync(d => d.MaDG == id);

            if (dg == null)
                return NotFound();

            _context.DanhGias.Remove(dg);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
