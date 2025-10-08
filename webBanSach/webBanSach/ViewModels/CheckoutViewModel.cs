using webBanSach.Models;

public class CheckoutViewModel
{
    public string HoTen { get; set; } = "";
    public string Email { get; set; } = "";
    public string DiaChiGiao { get; set; } = "";
    public string? SDT { get; set; }
    public List<GioHang> GioHangs { get; set; } = new List<GioHang>();
    public string? MaCode { get; set; }
    public decimal TongTien { get; set; }
    public decimal TongTienSauGiam { get; set; }
    public decimal? GiamGia { get; set; }
}
