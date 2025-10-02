using Microsoft.AspNetCore.Mvc;
using webBanSach.Models;

namespace webBanSach.ViewModels
{
    public class CheckoutViewModel
    {
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string DiaChiGiao { get; set; }

        // Thông tin giỏ hàng
        public List<GioHang> GioHangs { get; set; }
        public decimal TongTien { get; set; }
    }

}
