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
        public string DiaChiGiao { get; set; } = "";
        public List<CT_DonHang> ChiTiet { get; set; } = new List<CT_DonHang>();
        public string? MaCode { get; set; }
        public string SDT { get; set; }
        // Non-nullable, dùng 0 nếu null
        public decimal TongTien { get; set; }
        public decimal TongTienSauGiam { get; set; }
        public decimal GiamGia { get; set; } // Tổng tiền giảm: TongTien - TongTienSauGiam

        public string TrangThai { get; set; } = "";
        public DateTime NgayDat { get; set; }

        // ✅ Constructor không tham số
        public DonHangViewModel() { }

        // ✅ Constructor mapping từ DonHang entity
        public DonHangViewModel(DonHang donHang)
        {
            MaDH = donHang.MaDH;
            HoTen = donHang.MaNDNavigation.HoTen;
            Email = donHang.MaNDNavigation.Email;
            DiaChiGiao = donHang.DiaChiGiao ?? "";
            ChiTiet = donHang.CT_DonHangs.ToList();
            MaCode = donHang.MaKMNavigation?.MaCode;
            TongTien = donHang.TongTien ?? 0m;
            TongTienSauGiam = donHang.TongTienSauGiam ?? donHang.TongTien ?? 0m;
            GiamGia = TongTien - TongTienSauGiam;
            TrangThai = donHang.TrangThai ?? "";
            NgayDat = donHang.NgayDat ?? DateTime.Now;
        }
    }
}
