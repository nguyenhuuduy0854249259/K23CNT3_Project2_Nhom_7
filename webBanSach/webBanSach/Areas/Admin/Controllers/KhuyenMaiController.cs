using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webBanSach.Models;

namespace webBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhuyenMaiController : Controller
    {
        private readonly WebBanSachContext _context;
        public KhuyenMaiController(WebBanSachContext context)
        {
            _context = context;
        }

        // GET: Admin/KhuyenMai
        public async Task<IActionResult> Index(string? search)
        {
            var q = _context.KhuyenMais.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                q = q.Where(k => k.MaCode.Contains(search) || (k.MoTa ?? "").Contains(search));
            }

            var list = await q.OrderByDescending(k => k.NgayBatDau).ToListAsync();
            return View(list);
        }

        // GET: Admin/KhuyenMai/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var km = await _context.KhuyenMais.FirstOrDefaultAsync(k => k.MaKM == id);
            if (km == null) return NotFound();

            return View(km);
        }

        // GET: Admin/KhuyenMai/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/KhuyenMai/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhuyenMai km)
        {
            if (ModelState.IsValid)
            {
                _context.Add(km);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(km);
        }

        // GET: Admin/KhuyenMai/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var km = await _context.KhuyenMais.FindAsync(id);
            if (km == null) return NotFound();

            return View(km);
        }

        // POST: Admin/KhuyenMai/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KhuyenMai km)
        {
            if (id != km.MaKM) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(km);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(km);
        }

        // POST: Admin/KhuyenMai/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lh = await _context.KhuyenMais.FindAsync(id);
            if (lh != null)
            {
                _context.KhuyenMais.Remove(lh);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/KhuyenMai/Toggle/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            var km = await _context.KhuyenMais.FindAsync(id);
            if (km != null)
            {
                km.TrangThai = !(km.TrangThai ?? false);
                _context.Update(km);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
