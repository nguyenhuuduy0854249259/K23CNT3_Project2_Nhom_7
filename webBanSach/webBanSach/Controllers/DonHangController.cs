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
    public class DonHangController : Controller
    {
        private readonly WebBanSachContext _context;

        public DonHangController(WebBanSachContext context)
        {
            _context = context;
        }

        // ================================
        // Hiển thị danh sách đơn hàng của user
        // ================================
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("MaND");
            if (userId == null) return RedirectToAction("Login", "Account");

            var donHangs = await _context.DonHangs
                .Include(d => d.MaNDNavigation)
                .Include(d => d.MaKMNavigation)
                .Include(d => d.CT_DonHangs)
                    .ThenInclude(ct => ct.MaSachNavigation)
                .Where(d => d.MaND == userId.Value)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();

            // Chuyển sang ViewModel
            var vmList = donHangs.Select(d => new DonHangViewModel
            {
                MaDH = d.MaDH,
                HoTen = d.MaNDNavigation?.HoTen ?? "Không xác định",
                Email = d.MaNDNavigation?.Email ?? "",
                SDT = d.MaNDNavigation?.SDT ?? "",
                DiaChiGiao = d.DiaChiGiao ?? "",
                ChiTiet = d.CT_DonHangs?.ToList() ?? new List<CT_DonHang>(),
                MaCode = d.MaKMNavigation?.MaCode,
                TongTien = d.TongTien ?? 0m,
                TongTienSauGiam = d.TongTienSauGiam ?? d.TongTien ?? 0m,
                GiamGia = (d.TongTien ?? 0m) - (d.TongTienSauGiam ?? d.TongTien ?? 0m),
                TrangThai = d.TrangThai ?? "",
                NgayDat = d.NgayDat ?? DateTime.Now
            }).ToList();

            return View(vmList);
        }


        // ================================
        // Hiển thị chi tiết đơn hàng
        // ================================
        public async Task<IActionResult> Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("MaND");
            if (userId == null) return RedirectToAction("Login", "Account");

            var donHang = await _context.DonHangs
                .Include(d => d.CT_DonHangs)
                    .ThenInclude(ct => ct.MaSachNavigation)
                .Include(d => d.MaKMNavigation)
                .Include(d => d.MaNDNavigation)
                .FirstOrDefaultAsync(d => d.MaDH == id && d.MaND == userId.Value);

            if (donHang == null) return NotFound();

            // Sử dụng constructor mapping từ DonHang entity
            var donHangVM = new DonHangViewModel(donHang);

            return View(donHangVM);
        }

    }
}
