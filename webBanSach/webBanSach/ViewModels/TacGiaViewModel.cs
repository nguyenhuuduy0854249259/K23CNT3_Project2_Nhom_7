using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace webBanSach.ViewModels
{
    public class TacGiaViewModel
    {
        public int MaTG { get; set; }

        [Required(ErrorMessage = "Tên tác giả không được để trống")]
        [StringLength(100)]
        public string TenTG { get; set; }

        [StringLength(255)]
        public string? MoTa { get; set; }

        public string? HinhAnh { get; set; }

        [Display(Name = "Ảnh tác giả")]
        public IFormFile? HinhAnhFile { get; set; }
    }
}
