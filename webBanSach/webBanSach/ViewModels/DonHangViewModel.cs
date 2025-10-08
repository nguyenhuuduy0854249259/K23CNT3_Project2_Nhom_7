using webBanSach.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace webBanSach.ViewModels
{
    public class DonHangViewModel
    {
        public int MaDH { get; set; }
        public string HoTen { get; set; } = "";
        public string Email { get; set; } = "";
        public string SDT { get; set; } = ""; // ✅ Thêm SDT đầy đủ
        public string DiaChiGiao { get; set; } = "";
        public List<CT_DonHang> ChiTiet { get; set; } = new List<CT_DonHang>();
        public string? MaCode { get; set; }

        public decimal TongTien { get; set; }
        public decimal TongTienSauGiam { get; set; }
        public decimal GiamGia { get; set; } // = TongTien - TongTienSauGiam

        public string TrangThai { get; set; } = "";
        public DateTime NgayDat { get; set; }

        // ✅ Constructor mặc định
        public DonHangViewModel() { }

        // ✅ Constructor mapping từ entity DonHang
        public DonHangViewModel(DonHang donHang)
        {
            MaDH = donHang.MaDH;
            HoTen = donHang.MaNDNavigation?.HoTen ?? "Không xác định";
            Email = donHang.MaNDNavigation?.Email ?? "";
            SDT = donHang.MaNDNavigation?.SDT ?? "";  // ✅ Ánh xạ SĐT từ người dùng
            DiaChiGiao = donHang.DiaChiGiao ?? "";

            ChiTiet = donHang.CT_DonHangs?.ToList() ?? new List<CT_DonHang>();
            MaCode = donHang.MaKMNavigation?.MaCode;

            TongTien = donHang.TongTien ?? 0m;
            TongTienSauGiam = donHang.TongTienSauGiam ?? donHang.TongTien ?? 0m;
            GiamGia = TongTien - TongTienSauGiam;

            TrangThai = donHang.TrangThai ?? "Chờ xử lý";
            NgayDat = donHang.NgayDat ?? DateTime.Now;
        }
    }
}
