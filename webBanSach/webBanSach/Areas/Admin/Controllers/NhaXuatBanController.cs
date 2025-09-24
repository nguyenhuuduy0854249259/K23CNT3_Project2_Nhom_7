using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webBanSach.Models;

namespace webBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NhaXuatBanController : Controller
    {
        private readonly WebBanSachContext _context;

        public NhaXuatBanController(WebBanSachContext context)
        {
            _context = context;
        }

        // GET: Admin/NhaXuatBan
        public async Task<IActionResult> Index(string? searchString)
        {
            var query = _context.NhaXuatBans.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(n => n.TenNXB.Contains(searchString));
            }

            return View(await query.OrderBy(n => n.MaNXB).ToListAsync());
        }

        // GET: Admin/NhaXuatBan/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var nxb = await _context.NhaXuatBans
                .Include(n => n.Saches)
                .FirstOrDefaultAsync(m => m.MaNXB == id);

            if (nxb == null) return NotFound();

            return View(nxb);
        }

        // GET: Admin/NhaXuatBan/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/NhaXuatBan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenNXB,DiaChi,SDT,Email")] NhaXuatBan nxb)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nxb);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nxb);
        }

        // GET: Admin/NhaXuatBan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var nxb = await _context.NhaXuatBans.FindAsync(id);
            if (nxb == null) return NotFound();

            return View(nxb);
        }

        // POST: Admin/NhaXuatBan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaNXB,TenNXB,DiaChi,SDT,Email")] NhaXuatBan nxb)
        {
            if (id != nxb.MaNXB) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nxb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.NhaXuatBans.Any(e => e.MaNXB == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(nxb);
        }

        // GET: Admin/NhaXuatBan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var nxb = await _context.NhaXuatBans
                .FirstOrDefaultAsync(m => m.MaNXB == id);

            if (nxb == null) return NotFound();

            return View(nxb);
        }

        // POST: Admin/NhaXuatBan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nxb = await _context.NhaXuatBans.FindAsync(id);
            if (nxb != null)
            {
                _context.NhaXuatBans.Remove(nxb);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
