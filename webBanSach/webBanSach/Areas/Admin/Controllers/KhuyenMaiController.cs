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

            var km = await _context.KhuyenMais
                        .AsNoTracking()
                        .FirstOrDefaultAsync(k => k.MaKM == id);
            if (km == null) return NotFound();

            return View(km);
        }

        // GET: Admin/KhuyenMai/Create
        public IActionResult Create() => View();

        // POST: Admin/KhuyenMai/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhuyenMai km)
        {
            // Xử lý null theo LoaiGiam
            if (km.LoaiGiam == "PhanTram")
                km.SoTienGiam = null;
            else if (km.LoaiGiam == "TienMat")
                km.PhanTramGiam = null;

            if (!ModelState.IsValid)
                return View(km);

            try
            {
                _context.Add(km);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm khuyến mãi thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Tạo mới thất bại. " + ex.Message);
                return View(km);
            }
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

            // Xử lý null theo LoaiGiam
            if (km.LoaiGiam == "PhanTram")
                km.SoTienGiam = null;
            else if (km.LoaiGiam == "TienMat")
                km.PhanTramGiam = null;

            if (!ModelState.IsValid)
                return View(km);

            try
            {
                _context.Update(km);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật khuyến mãi thành công!";
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Cập nhật thất bại. " + ex.Message);
                return View(km);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/KhuyenMai/Delete/5
        // POST: Admin/KhuyenMai/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var km = await _context.KhuyenMais.FindAsync(id);
            if (km != null)
            {
                try
                {
                    _context.KhuyenMais.Remove(km);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Xóa khuyến mãi '{km.MaCode}' thành công!";
                }
                catch (DbUpdateException ex)
                {
                    TempData["Error"] = $"Xóa thất bại. {ex.Message}";
                }
            }
            else
            {
                TempData["Error"] = "Khuyến mãi không tồn tại.";
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
                km.TrangThai = !km.TrangThai;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
