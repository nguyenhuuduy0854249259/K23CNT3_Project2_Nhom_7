using System.ComponentModel.DataAnnotations;

namespace webBanSach.ViewModels
{
    public class ProfileVM
    {
        public int MaND { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        public string HoTen { get; set; }

        [EmailAddress]
        public string Email { get; set; }  // Email không cho sửa

        [Phone]
        public string SDT { get; set; }

        public string DiaChi { get; set; }

        public string? HinhAnh { get; set; }

        // Dùng khi đổi mật khẩu
        [DataType(DataType.Password)]
        public string? MatKhauCu { get; set; }

        [DataType(DataType.Password)]
        public string? MatKhauMoi { get; set; }

        [DataType(DataType.Password)]
        [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string? XacNhanMatKhau { get; set; }
    }
}
