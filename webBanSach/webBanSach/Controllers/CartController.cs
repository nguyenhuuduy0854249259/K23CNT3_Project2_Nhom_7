using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webBanSach.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using webBanSach.ViewModels;

namespace webBanSach.Controllers
{
    public class CartController : Controller
    {
        private readonly WebBanSachContext _context;

        public CartController(WebBanSachContext context)
        {
            _context = context;
        }

        // ================================
        // Hiển thị giỏ hàng
        // ================================
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("MaND");
            if (userId == null) return RedirectToAction("Login", "Account");

            var gioHang = await _context.GioHangs
                .Include(g => g.MaSachNavigation)
                .Where(g => g.MaND == userId.Value)
                .ToListAsync();

            return View(gioHang);
        }

        // ================================
        // Thêm sản phẩm vào giỏ
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int maSach, int soLuong = 1)
        {
            var userId = HttpContext.Session.GetInt32("MaND");
            if (userId == null) return RedirectToAction("Login", "Account");

            var cartItem = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.MaND == userId && g.MaSach == maSach);

            if (cartItem != null)
            {
                cartItem.SoLuong += soLuong;
                _context.GioHangs.Update(cartItem);
            }
            else
            {
                _context.GioHangs.Add(new GioHang
                {
                    MaND = userId.Value,
                    MaSach = maSach,
                    SoLuong = soLuong
                });
            }

            await _context.SaveChangesAsync();

            // Cập nhật CartCount trong session để layout hiển thị số lượng
            var cartCount = await _context.GioHangs
                .Where(g => g.MaND == userId.Value)
                .SumAsync(g => g.SoLuong);
            HttpContext.Session.SetInt32("CartCount", cartCount);

            TempData["Message"] = "✅ Sản phẩm đã được thêm vào giỏ hàng!";
            return Redirect(Request.Headers["Referer"].ToString()); // quay về trang trước
        }

        // ================================
        // Cập nhật số lượng
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int maSach, int soLuong)
        {
            var userId = HttpContext.Session.GetInt32("MaND");
            if (userId == null) return RedirectToAction("Login", "Account");

            var cartItem = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.MaND == userId && g.MaSach == maSach);

            if (cartItem != null)
            {
                cartItem.SoLuong = soLuong;
                _context.GioHangs.Update(cartItem);
                await _context.SaveChangesAsync();
            }

            // Cập nhật CartCount
            var cartCount = await _context.GioHangs
                .Where(g => g.MaND == userId.Value)
                .SumAsync(g => g.SoLuong);
            HttpContext.Session.SetInt32("CartCount", cartCount);

            return RedirectToAction("Index");
        }

        // ================================
        // Xóa sản phẩm khỏi giỏ
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int maSach)
        {
            var userId = HttpContext.Session.GetInt32("MaND");
            if (userId == null) return RedirectToAction("Login", "Account");

            var cartItem = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.MaND == userId && g.MaSach == maSach);

            if (cartItem != null)
            {
                _context.GioHangs.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            // Cập nhật CartCount
            var cartCount = await _context.GioHangs
                .Where(g => g.MaND == userId.Value)
                .SumAsync(g => g.SoLuong);
            HttpContext.Session.SetInt32("CartCount", cartCount);

            return RedirectToAction("Index");
        }

        // ================================
        // GET Checkout
        // ================================
        public async Task<IActionResult> Checkout()
        {
            var userId = HttpContext.Session.GetInt32("MaND");
            if (userId == null) return RedirectToAction("Login", "Account");

            var nguoiDung = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.MaND == userId.Value);

            var gioHang = await _context.GioHangs
                .Include(g => g.MaSachNavigation)
                .Where(g => g.MaND == userId.Value)
                .ToListAsync();

            if (nguoiDung == null) return RedirectToAction("Index");

            var vm = new CheckoutViewModel
            {
                HoTen = nguoiDung.HoTen,
                Email = nguoiDung.Email,
                GioHangs = gioHang,
                TongTien = gioHang.Sum(g => g.SoLuong * g.MaSachNavigation.GiaBan)
            };

            return View(vm);
        }

        // ================================
        // POST Checkout
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("MaND");
            if (userId == null) return RedirectToAction("Login", "Account");

            var gioHang = await _context.GioHangs
                .Include(g => g.MaSachNavigation)
                .Where(g => g.MaND == userId.Value)
                .ToListAsync();

            if (!gioHang.Any())
            {
                TempData["Message"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            // Tạo đơn hàng
            var donHang = new DonHang
            {
                MaND = userId.Value,
                NgayDat = DateTime.Now,
                TongTien = gioHang.Sum(g => g.SoLuong * g.MaSachNavigation.GiaBan),
                TrangThai = "Chờ xử lý",
                DiaChiGiao = model.DiaChiGiao
            };

            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();

            foreach (var item in gioHang)
            {
                _context.CT_DonHangs.Add(new CT_DonHang
                {
                    MaDH = donHang.MaDH,
                    MaSach = item.MaSach,
                    SoLuong = item.SoLuong,
                    DonGia = item.MaSachNavigation.GiaBan
                });
            }

            _context.GioHangs.RemoveRange(gioHang);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetInt32("CartCount", 0);

            TempData["Message"] = "✅ Thanh toán thành công!";
            return RedirectToAction("Index", "DonHang");
        }

    }
}
