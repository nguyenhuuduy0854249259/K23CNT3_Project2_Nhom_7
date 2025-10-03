using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using webBanSach.Models;
using webBanSach.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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

            if (cartItem != null) cartItem.SoLuong += soLuong;
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

            // Cập nhật session CartCount
            var cartCount = await _context.GioHangs
                .Where(g => g.MaND == userId.Value)
                .SumAsync(g => g.SoLuong);
            HttpContext.Session.SetInt32("CartCount", cartCount);

            TempData["Message"] = "✅ Đã thêm vào giỏ hàng!";
            return Redirect(Request.Headers["Referer"].ToString());
        }
        // Cập nhật số lượng sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int maSach, int soLuong)
        {
            var userId = HttpContext.Session.GetInt32("MaND");
            if (userId == null) return RedirectToAction("Login", "Account");

            var cartItem = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.MaND == userId.Value && g.MaSach == maSach);

            if (cartItem != null)
            {
                if (soLuong <= 0)
                {
                    // Nếu số lượng <=0, xóa sản phẩm khỏi giỏ
                    _context.GioHangs.Remove(cartItem);
                }
                else
                {
                    cartItem.SoLuong = soLuong;
                    _context.GioHangs.Update(cartItem);
                }

                await _context.SaveChangesAsync();

                // Cập nhật lại session CartCount
                var cartCount = await _context.GioHangs
                    .Where(g => g.MaND == userId.Value)
                    .SumAsync(g => g.SoLuong);
                HttpContext.Session.SetInt32("CartCount", cartCount);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int maSach)
        {
            var userId = HttpContext.Session.GetInt32("MaND");
            if (userId == null) return RedirectToAction("Login", "Account");

            var cartItem = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.MaND == userId.Value && g.MaSach == maSach);

            if (cartItem != null)
            {
                _context.GioHangs.Remove(cartItem);
                await _context.SaveChangesAsync();

                // Cập nhật lại session CartCount
                var cartCount = await _context.GioHangs
                    .Where(g => g.MaND == userId.Value)
                    .SumAsync(g => g.SoLuong);
                HttpContext.Session.SetInt32("CartCount", cartCount);
            }

            return RedirectToAction("Index");
        }

        // ================================
        // Áp dụng mã giảm giá
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApplyCoupon(string maCode)
        {
            HttpContext.Session.SetString("MaKhuyenMai", maCode);
            return RedirectToAction("Checkout");
        }

        // ================================
        // GET Checkout
        // ================================
        public async Task<IActionResult> Checkout()
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

            decimal tongTien = gioHang.Sum(g => g.SoLuong * g.MaSachNavigation.GiaBan);
            decimal giamGia = 0m;
            string? maCode = HttpContext.Session.GetString("MaKhuyenMai");

            if (!string.IsNullOrEmpty(maCode))
            {
                var km = await _context.KhuyenMais
                    .FirstOrDefaultAsync(k => k.MaCode == maCode && k.TrangThai == true &&
                                              k.NgayBatDau <= DateTime.Now && k.NgayKetThuc >= DateTime.Now);
                if (km != null)
                {
                    if (km.LoaiGiam == "PhanTram" && km.PhanTramGiam.HasValue)
                    {
                        giamGia = tongTien * (km.PhanTramGiam.Value / 100);
                        if (km.GiamToiDa.HasValue && giamGia > km.GiamToiDa.Value)
                            giamGia = km.GiamToiDa.Value;
                    }
                    else if (km.LoaiGiam == "TienMat" && km.SoTienGiam.HasValue)
                    {
                        giamGia = km.SoTienGiam.Value;
                    }
                }
            }

            decimal tongTienSauGiam = Math.Max(tongTien - giamGia, 0);

            var nguoiDung = await _context.NguoiDungs.FindAsync(userId.Value);

            var vm = new CheckoutViewModel
            {
                HoTen = nguoiDung?.HoTen ?? "",
                Email = nguoiDung?.Email ?? "",
                DiaChiGiao = "",
                GioHangs = gioHang,
                MaCode = maCode,
                TongTien = tongTien,
                GiamGia = giamGia,
                TongTienSauGiam = tongTienSauGiam
            };

            return View(vm);
        }

        // ================================
        // POST Checkout - tạo DonHang
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

            // --- Kiểm tra đủ số lượng ---
            foreach (var item in gioHang)
            {
                if (item.MaSachNavigation.SoLuong < item.SoLuong)
                {
                    TempData["Error"] = $"Sách '{item.MaSachNavigation.TenSach}' chỉ còn {item.MaSachNavigation.SoLuong} cuốn, không đủ số lượng bạn đặt.";
                    return RedirectToAction("Index", "Cart");
                }
            }

            decimal tongTien = gioHang.Sum(g => g.SoLuong * g.MaSachNavigation.GiaBan);
            decimal giamGia = 0m;
            int? maKM = null;

            string? maCode = model.MaCode ?? HttpContext.Session.GetString("MaKhuyenMai");
            if (!string.IsNullOrEmpty(maCode))
            {
                var km = await _context.KhuyenMais
                    .FirstOrDefaultAsync(k => k.MaCode == maCode && k.TrangThai == true &&
                                              k.NgayBatDau <= DateTime.Now && k.NgayKetThuc >= DateTime.Now);
                if (km != null)
                {
                    if (km.LoaiGiam == "PhanTram" && km.PhanTramGiam.HasValue)
                    {
                        giamGia = tongTien * (km.PhanTramGiam.Value / 100);
                        if (km.GiamToiDa.HasValue && giamGia > km.GiamToiDa.Value)
                            giamGia = km.GiamToiDa.Value;
                    }
                    else if (km.LoaiGiam == "TienMat" && km.SoTienGiam.HasValue)
                    {
                        giamGia = km.SoTienGiam.Value;
                    }
                    maKM = km.MaKM;
                }
            }

            decimal tongTienSauGiam = Math.Max(tongTien - giamGia, 0);
            decimal tyLeGiam = tongTien > 0 ? tongTienSauGiam / tongTien : 1;

            var donHang = new DonHang
            {
                MaND = userId.Value,
                NgayDat = DateTime.Now,
                TongTien = tongTien,
                TongTienSauGiam = tongTienSauGiam,
                TrangThai = "Chờ xử lý",
                DiaChiGiao = string.IsNullOrEmpty(model.DiaChiGiao) ? "Chưa cung cấp" : model.DiaChiGiao,
                MaKM = maKM
            };
            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();

            foreach (var item in gioHang)
            {
                // --- Trừ số lượng sách ---
                item.MaSachNavigation.SoLuong -= item.SoLuong;
                if (item.MaSachNavigation.SoLuong < 0) item.MaSachNavigation.SoLuong = 0;
                _context.Saches.Update(item.MaSachNavigation);

                decimal donGiaSauGiam = Math.Round(item.MaSachNavigation.GiaBan * tyLeGiam, 0);

                _context.CT_DonHangs.Add(new CT_DonHang
                {
                    MaDH = donHang.MaDH,
                    MaSach = item.MaSach,
                    SoLuong = item.SoLuong,
                    DonGia = donGiaSauGiam
                });
            }

            _context.GioHangs.RemoveRange(gioHang);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetInt32("CartCount", 0);
            HttpContext.Session.Remove("MaKhuyenMai");

            TempData["SuccessMessage"] = "✅ Thanh toán thành công!";
            return RedirectToAction("Index", "DonHang");
        }

        // ================================
        // Mua ngay: thêm 1 sản phẩm và chuyển thẳng tới Checkout
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckoutNow(int maSach, int soLuong = 1)
        {
            var userId = HttpContext.Session.GetInt32("MaND");
            if (userId == null) return RedirectToAction("Login", "Account");

            // Lấy sách
            var sach = await _context.Saches.FindAsync(maSach);
            if (sach == null || (sach.SoLuong ?? 0) < soLuong)
            {
                TempData["Error"] = "Sách không đủ số lượng hoặc không tồn tại!";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            // Xóa giỏ hàng hiện tại của user để chỉ mua sản phẩm này
            var currentCart = await _context.GioHangs.Where(g => g.MaND == userId.Value).ToListAsync();
            _context.GioHangs.RemoveRange(currentCart);

            // Thêm sản phẩm mua ngay
            _context.GioHangs.Add(new GioHang
            {
                MaND = userId.Value,
                MaSach = maSach,
                SoLuong = soLuong
            });
            await _context.SaveChangesAsync();

            // Cập nhật CartCount
            HttpContext.Session.SetInt32("CartCount", soLuong);

            // Chuyển thẳng tới Checkout
            return RedirectToAction("Checkout");
        }

    }


}
