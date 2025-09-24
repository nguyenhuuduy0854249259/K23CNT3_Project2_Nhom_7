using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webBanSach.Models;

namespace webBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DonHangController : Controller
    {
        private readonly WebBanSachContext _context;
        public DonHangController(WebBanSachContext context)
        {
            _context = context;
        }

        // GET: Admin/DonHang
        public async Task<IActionResult> Index(string? searchString, string? status)
        {
            var query = _context.DonHangs
                        .Include(d => d.MaNDNavigation)
                        .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(d => d.MaDH.ToString().Contains(searchString)
                                      || d.MaNDNavigation.HoTen.Contains(searchString));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(d => d.TrangThai == status);
            }

            var list = await query.OrderByDescending(d => d.NgayDat).ToListAsync();
            ViewBag.StatusList = OrderStatus.All;
            return View(list);
        }

        // GET: Admin/DonHang/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var don = await _context.DonHangs
                .Include(d => d.MaNDNavigation)
                .Include(d => d.CT_DonHangs)
                    .ThenInclude(ct => ct.MaSachNavigation)
                .FirstOrDefaultAsync(d => d.MaDH == id);

            if (don == null) return NotFound();
            ViewBag.StatusList = OrderStatus.All;
            return View(don);
        }

        // POST: Admin/DonHang/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string trangThai, string returnTo = "Index")
        {
            var don = await _context.DonHangs.FindAsync(id);
            if (don == null) return NotFound();

            // Validate nghiệp vụ: không đổi trạng thái nếu đã hoàn tất / hủy
            if (don.TrangThai == OrderStatus.HoanTat || don.TrangThai == OrderStatus.Huy)
            {
                TempData["Error"] = "Đơn hàng đã hoàn tất hoặc bị hủy, không thể thay đổi!";
                return RedirectToAction(returnTo, new { id });
            }

            don.TrangThai = trangThai;
         
            _context.Update(don);
            await _context.SaveChangesAsync();

            if (returnTo == "Details")
                return RedirectToAction("Details", new { id });
            return RedirectToAction("Index");
        }

        // POST: Admin/DonHang/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var don = await _context.DonHangs.FindAsync(id);
            if (don == null) return NotFound();

            if (don.TrangThai == OrderStatus.HoanTat)
            {
                TempData["Error"] = "Đơn hàng đã hoàn tất, không thể hủy!";
                return RedirectToAction("Details", new { id });
            }

            don.TrangThai = OrderStatus.Huy;
 
            _context.Update(don);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Admin/DonHang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var don = await _context.DonHangs.FindAsync(id);
            if (don != null && don.TrangThai == "Hủy") // chỉ xóa đơn Hủy
            {
                _context.DonHangs.Remove(don);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
