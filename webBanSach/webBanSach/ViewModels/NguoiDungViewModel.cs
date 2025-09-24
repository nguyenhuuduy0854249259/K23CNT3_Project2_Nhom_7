using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace webBanSach.ViewModels
{
    public class NguoiDungViewModel
    {
        public int MaND { get; set; }

        [Required, StringLength(100)]
        public string HoTen { get; set; } = null!;

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = null!;

        [DataType(DataType.Password)]
        public string? MatKhau { get; set; }

        [StringLength(15)]
        public string? SDT { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }

        public string? HinhAnh { get; set; }
        public IFormFile? HinhAnhFile { get; set; }

        [StringLength(20)]
        public string? LoaiNguoiDung { get; set; }

        [StringLength(50)]
        public string? TrangThai { get; set; }
    }
}
