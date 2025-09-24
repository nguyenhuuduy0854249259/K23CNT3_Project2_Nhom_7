using System.ComponentModel.DataAnnotations;

namespace webBanSach.ViewModels
{
    public class AdminLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; } = null!;
    }
}
