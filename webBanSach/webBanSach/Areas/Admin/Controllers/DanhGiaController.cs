using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webBanSach.Helpers;
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
        public async Task<IActionResult> Index(string? search, int pageNumber = 1)
        {
            int pageSize = 10;

            var query = _context.DanhGias
                .Include(d => d.MaNDNavigation)
                .Include(d => d.MaSachNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(d =>
                    d.MaNDNavigation.HoTen.Contains(search) ||
                    d.MaSachNavigation.TenSach.Contains(search));
            }

            query = query.OrderByDescending(d => d.NgayDG);

            var pagedList = await PaginatedList<DanhGia>.CreateAsync(query, pageNumber, pageSize);
            return View(pagedList);
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
