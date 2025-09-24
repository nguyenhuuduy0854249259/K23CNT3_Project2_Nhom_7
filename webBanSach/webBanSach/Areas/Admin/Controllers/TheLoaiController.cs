using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webBanSach.Models;

namespace webBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TheLoaiController : Controller
    {
        private readonly WebBanSachContext _context;

        public TheLoaiController(WebBanSachContext context)
        {
            _context = context;
        }

        // GET: Admin/TheLoai
        // GET: Admin/TheLoai
        public async Task<IActionResult> Index(string? searchString)
        {
            var query = _context.TheLoais.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(l => l.TenLoai.Contains(searchString));
            }

            // ✅ Sắp xếp theo MaLoai tăng dần
            query = query.OrderBy(l => l.MaLoai);

            return View(await query.ToListAsync());
        }


        // GET: Admin/TheLoai/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var theLoai = await _context.TheLoais
                .FirstOrDefaultAsync(m => m.MaLoai == id);

            if (theLoai == null) return NotFound();

            return View(theLoai);
        }

        // GET: Admin/TheLoai/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/TheLoai/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TheLoai theLoai)
        {
            if (ModelState.IsValid)
            {
                _context.Add(theLoai);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(theLoai);
        }

        // GET: Admin/TheLoai/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var theLoai = await _context.TheLoais.FindAsync(id);
            if (theLoai == null) return NotFound();

            return View(theLoai);
        }

        // POST: Admin/TheLoai/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TheLoai theLoai)
        {
            if (id != theLoai.MaLoai) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(theLoai);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TheLoaiExists(theLoai.MaLoai))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(theLoai);
        }

        // POST: Admin/TheLoai/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var theLoai = await _context.TheLoais.FindAsync(id);
            if (theLoai != null)
            {
                _context.TheLoais.Remove(theLoai);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TheLoaiExists(int id)
        {
            return _context.TheLoais.Any(e => e.MaLoai == id);
        }
    }
}
