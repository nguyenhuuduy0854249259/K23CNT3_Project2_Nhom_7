using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using webBanSach.Models;

namespace webBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DonHangController : Controller
    {
        private readonly WebBanSachContext _context;
        private const int PageSize = 10; // Số bản ghi mỗi trang
        public DonHangController(WebBanSachContext context)
        {
            _context = context;
        }

        // GET: Admin/DonHang
        public async Task<IActionResult> Index(string searchString, string status, int page = 1)
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

            query = query.OrderByDescending(d => d.NgayDat);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            var items = await query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.StatusList = OrderStatus.All;
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(items);
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

            // Không cho đổi trạng thái nếu đã Hoàn tất hoặc Hủy
            if (don.TrangThai == OrderStatus.HoanTat || don.TrangThai == OrderStatus.Huy)
            {
                TempData["Error"] = "Đơn hàng đã hoàn tất hoặc bị hủy, không thể thay đổi!";
                return RedirectToAction(returnTo, new { id });
            }

            don.TrangThai = trangThai;
            _context.Update(don);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đơn hàng {id} đã được cập nhật trạng thái.";

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

            TempData["Success"] = $"Đơn hàng {id} đã bị hủy.";
            return RedirectToAction("Details", new { id });
        }

        // POST: Admin/DonHang/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var don = await _context.DonHangs
                        .Include(d => d.CT_DonHangs)
                        .FirstOrDefaultAsync(d => d.MaDH == id);

            if (don != null && don.TrangThai == OrderStatus.Huy)
            {
                // Xóa chi tiết trước để tránh FK
                _context.CT_DonHangs.RemoveRange(don.CT_DonHangs);

                // Xóa đơn hàng
                _context.DonHangs.Remove(don);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Đơn hàng {id} đã được xóa.";
            }
            else
            {
                TempData["Error"] = $"Đơn hàng {id} không thể xóa (chỉ xóa đơn Hủy).";
            }

            return RedirectToAction("Index");
        }
    }
}
