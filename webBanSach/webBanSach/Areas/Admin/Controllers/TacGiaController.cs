using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using webBanSach.Helpers;
using webBanSach.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace webBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TacGiaController : Controller
    {
        private readonly WebBanSachContext _context;

        public TacGiaController(WebBanSachContext context)
        {
            _context = context;
        }

        // GET: Admin/TacGia
        public async Task<IActionResult> Index(string? searchString, int pageNumber = 1)
        {
            int pageSize = 10; // số item mỗi trang
            var query = _context.TacGias.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(n => n.TenTG.Contains(searchString));
            }
            var pagedList = await PaginatedList<TacGia>.CreateAsync(query, pageNumber, pageSize);
            return View(pagedList);
        }

        // GET: Admin/TacGia/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var tg = await _context.TacGias.FindAsync(id);
            if (tg == null) return NotFound();
            return View(tg);
        }

        // GET: Admin/TacGia/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/TacGia/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenTG")] TacGia tg)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tg);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tg);
        }

        // GET: Admin/TacGia/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var tg = await _context.TacGias.FindAsync(id);
            if (tg == null) return NotFound();
            return View(tg);
        }

        // POST: Admin/TacGia/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaTG,TenTG")] TacGia tg)
        {
            if (id != tg.MaTG) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tg);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TacGias.Any(e => e.MaTG == tg.MaTG))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tg);
        }

        // POST: Admin/TacGia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tg = await _context.TacGias.FindAsync(id);
            if (tg != null)
            {
                _context.TacGias.Remove(tg);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
