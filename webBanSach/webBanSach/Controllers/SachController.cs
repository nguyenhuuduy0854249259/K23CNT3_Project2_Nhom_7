using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webBanSach.Models;
using webBanSach.ViewModels;

namespace webBanSach.Controllers
{
    public class SachController : Controller
    {
        private readonly WebBanSachContext _context;

        public SachController(WebBanSachContext context)
        {
            _context = context;
        }

        // ========================
        // Index - danh sách sách
        // ========================
        public async Task<IActionResult> Index(string? keyword)
        {
            var query = _context.Saches
                .Include(s => s.MaNXBNavigation)
                .Include(s => s.Sach_TheLoais).ThenInclude(stl => stl.MaLoaiNavigation)
                .Include(s => s.Sach_TacGias).ThenInclude(stg => stg.MaTGNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s => s.TenSach.Contains(keyword));
                ViewBag.Keyword = keyword;
            }
            else
            {
                ViewBag.Keyword = "";
            }

            var listSach = await query
                .Select(s => new SachViewModel
                {
                    MaSach = s.MaSach,
                    TenSach = s.TenSach,
                    GiaBan = s.GiaBan,
                    SoLuong = s.SoLuong,
                    MoTa = s.MoTa,
                    HinhAnh = s.HinhAnh,
                    NXB = s.MaNXBNavigation.TenNXB,
                    TheLoaiNames = s.Sach_TheLoais.Select(st => st.MaLoaiNavigation.TenLoai),
                    TacGiaNames = s.Sach_TacGias.Select(st => st.MaTGNavigation.TenTG)
                })
                .ToListAsync();

            // Load dữ liệu dropdown để filter
            ViewBag.TheLoaiList = await _context.TheLoais.Select(t => t.TenLoai).ToListAsync();
            ViewBag.TacGiaList = await _context.TacGias.Select(t => t.TenTG).ToListAsync();
            ViewBag.NXBList = await _context.NhaXuatBans.Select(n => n.TenNXB).ToListAsync();

            return View(listSach);
        }

        // ========================
        // Chi tiết sách
        // ========================
        public async Task<IActionResult> Details(int? id, string? returnUrl)
        {
            if (id == null) return NotFound();

            var sach = await _context.Saches
                .Include(s => s.MaNXBNavigation)
                .Include(s => s.Sach_TheLoais).ThenInclude(stl => stl.MaLoaiNavigation)
                .Include(s => s.Sach_TacGias).ThenInclude(stg => stg.MaTGNavigation)
                .FirstOrDefaultAsync(s => s.MaSach == id);

            if (sach == null) return NotFound();

            sach.LuotXem++;
            _context.Update(sach);
            await _context.SaveChangesAsync();

            ViewBag.ReturnUrl = string.IsNullOrEmpty(returnUrl)
                ? Url.Action("Index", "Sach")
                : returnUrl;

            return View(sach);
        }

        // ========================
        // Search theo filter (theloai, tacgia, nxb)
        // ========================
        public async Task<IActionResult> Search(string filterType, string keyword)
        {
            if (string.IsNullOrEmpty(filterType) || string.IsNullOrEmpty(keyword))
                return RedirectToAction("Index");

            var query = _context.Saches
                .Include(s => s.MaNXBNavigation)
                .Include(s => s.Sach_TheLoais).ThenInclude(stl => stl.MaLoaiNavigation)
                .Include(s => s.Sach_TacGias).ThenInclude(stg => stg.MaTGNavigation)
                .AsQueryable();

            switch (filterType.ToLower())
            {
                case "theloai":
                    query = query.Where(s => s.Sach_TheLoais.Any(st => st.MaLoaiNavigation.TenLoai == keyword));
                    break;

                case "tacgia":
                    query = query.Where(s => s.Sach_TacGias.Any(st => st.MaTGNavigation.TenTG == keyword));
                    break;

                case "nxb":
                    query = query.Where(s => s.MaNXBNavigation.TenNXB == keyword);
                    break;
            }

            var result = await query
                .Select(s => new SachViewModel
                {
                    MaSach = s.MaSach,
                    TenSach = s.TenSach,
                    GiaBan = s.GiaBan,
                    SoLuong = s.SoLuong,
                    MoTa = s.MoTa,
                    HinhAnh = s.HinhAnh,
                    NXB = s.MaNXBNavigation.TenNXB,
                    TheLoaiNames = s.Sach_TheLoais.Select(st => st.MaLoaiNavigation.TenLoai),
                    TacGiaNames = s.Sach_TacGias.Select(st => st.MaTGNavigation.TenTG)
                })
                .ToListAsync();

            ViewBag.FilterType = filterType;
            ViewBag.Keyword = keyword;

            // Load dropdown để filter lại
            ViewBag.TheLoaiList = await _context.TheLoais.Select(t => t.TenLoai).ToListAsync();
            ViewBag.TacGiaList = await _context.TacGias.Select(t => t.TenTG).ToListAsync();
            ViewBag.NXBList = await _context.NhaXuatBans.Select(n => n.TenNXB).ToListAsync();

            return View("SearchResult", result);
        }
    }
}
