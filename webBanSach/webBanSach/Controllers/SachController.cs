using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webBanSach.Models;

namespace webBanSach.Controllers
{
    public class SachController : Controller
    {
        private readonly WebBanSachContext _context;

        public SachController(WebBanSachContext context)
        {
            _context = context;
        }

        // GET: Sach
        public async Task<IActionResult> Index()
        {
            var listSach = await _context.Saches
                .Include(s => s.MaNXBNavigation) // load thêm NXB
                .ToListAsync();
            return View(listSach);
        }

        // GET: Sach/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sach = await _context.Saches
                .Include(s => s.MaNXBNavigation)
                .FirstOrDefaultAsync(m => m.MaSach == id);

            if (sach == null) return NotFound();

            return View(sach);
        }

        // GET: Sach/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sach/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSach,TenSach,MaNXB,NamXB,GiaBan,SoLuong,MoTa,HinhAnh")] Sach sach)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sach);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sach);
        }

        // GET: Sach/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sach = await _context.Saches.FindAsync(id);
            if (sach == null) return NotFound();

            return View(sach);
        }

        // POST: Sach/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaSach,TenSach,MaNXB,NamXB,GiaBan,SoLuong,MoTa,HinhAnh")] Sach sach)
        {
            if (id != sach.MaSach) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sach);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SachExists(sach.MaSach)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sach);
        }

        // GET: Sach/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var sach = await _context.Saches
                .FirstOrDefaultAsync(m => m.MaSach == id);
            if (sach == null) return NotFound();

            return View(sach);
        }

        // POST: Sach/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sach = await _context.Saches.FindAsync(id);
            if (sach != null)
            {
                _context.Saches.Remove(sach);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SachExists(int id)
        {
            return _context.Saches.Any(e => e.MaSach == id);
        }
    }
}
