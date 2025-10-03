using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using webBanSach.Models;

namespace webBanSach.ViewModels
{
    public class SachViewModel
    {
        public int MaSach { get; set; }

        [Required(ErrorMessage = "Tên sách không được để trống")]
        [StringLength(200, ErrorMessage = "Tên sách tối đa 200 ký tự")]
        public string TenSach { get; set; } = null!;

        [Display(Name = "Nhà xuất bản")]
        public int MaNXB { get; set; }

        [Display(Name = "Năm xuất bản")]
        public int? NamXB { get; set; }

        [Required(ErrorMessage = "Giá bán không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Giá bán")]
        public decimal GiaBan { get; set; }

        [Display(Name = "Số lượng")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không hợp lệ")]
        public int? SoLuong { get; set; }

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Ảnh bìa")]
        public string? HinhAnh { get; set; }

        // ===== Thêm lượt xem =====
        [Display(Name = "Lượt xem")]
        public int LuotXem { get; set; }

        // Danh sách đánh giá
        public List<DanhGia>? DanhGias { get; set; }
        // Trong SachViewModel
        [Range(1, 5, ErrorMessage = "Điểm đánh giá từ 1 đến 5")]
        public int? DiemMoi { get; set; }

        [Display(Name = "Nhận xét")]
        public string? BinhLuanMoi { get; set; }

        // Dùng khi upload ảnh
        public IFormFile? HinhAnhFile { get; set; }

        // Khi hiển thị sách (Index / Search)
        public string? NXB { get; set; }
        public IEnumerable<string>? TheLoaiNames { get; set; }
        public IEnumerable<string>? TacGiaNames { get; set; }

        // Dropdown list cho Create/Edit
        public IEnumerable<SelectListItem>? NhaXuatBans { get; set; }
        public IEnumerable<SelectListItem>? TheLoais { get; set; }
        public IEnumerable<SelectListItem>? TacGias { get; set; }

        // Lưu lựa chọn từ form
        public List<int>? SelectedTheLoai { get; set; }
        public List<int>? SelectedTacGia { get; set; }
    }
}
