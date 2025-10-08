using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webBanSach.Models;
using webBanSach.ViewModels;

namespace webBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NguoiDungController : Controller
    {
        private readonly WebBanSachContext _context;
        private readonly IWebHostEnvironment _env;

        public NguoiDungController(WebBanSachContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Admin/NguoiDung
        public async Task<IActionResult> Index(string? searchString, string? loaiNguoiDung)
        {
            // Truy vấn ban đầu
            var query = _context.NguoiDungs.AsQueryable();

            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(n => n.HoTen.Contains(searchString));
            }

            // Lọc theo loại người dùng (Admin hoặc User)
            if (!string.IsNullOrEmpty(loaiNguoiDung))
            {
                query = query.Where(n => n.LoaiNguoiDung == loaiNguoiDung);
            }

            // Lưu lại giá trị filter để view hiển thị lại
            ViewData["CurrentFilter"] = searchString;
            ViewData["LoaiNguoiDung"] = loaiNguoiDung;

            // Trả về danh sách sắp xếp theo Mã người dùng
            return View(await query.OrderBy(n => n.MaND).ToListAsync());
        }


        // GET: Admin/NguoiDung/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var nd = await _context.NguoiDungs.FirstOrDefaultAsync(m => m.MaND == id);
            if (nd == null) return NotFound();
            return View(nd);
        }

        // GET: Admin/NguoiDung/Create
        public IActionResult Create() => View();

        // POST: Admin/NguoiDung/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NguoiDungViewModel vm)
        {
            if (ModelState.IsValid)
            {
                string? fileName = null;
                if (vm.HinhAnhFile != null && vm.HinhAnhFile.Length > 0)
                {
                    string folder = Path.Combine(_env.WebRootPath, "images", "nguoidung");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.HinhAnhFile.FileName);
                    string path = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await vm.HinhAnhFile.CopyToAsync(stream);
                    }
                }

                var nd = new NguoiDung
                {
                    HoTen = vm.HoTen,
                    Email = vm.Email,
                    MatKhau = BCrypt.Net.BCrypt.HashPassword(vm.MatKhau), // hash mật khẩu
                    SDT = vm.SDT,
                    DiaChi = vm.DiaChi,
                    HinhAnh = fileName,
                    LoaiNguoiDung = vm.LoaiNguoiDung,
                    TrangThai = "Hoạt động",
                    NgayTao = DateTime.Now
                };

                _context.Add(nd);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        // GET: Admin/NguoiDung/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var nd = await _context.NguoiDungs.FindAsync(id);
            if (nd == null) return NotFound();

            var vm = new NguoiDungViewModel
            {
                MaND = nd.MaND,
                HoTen = nd.HoTen,
                Email = nd.Email,
                SDT = nd.SDT,
                DiaChi = nd.DiaChi,
                HinhAnh = nd.HinhAnh,
                LoaiNguoiDung = nd.LoaiNguoiDung,
                TrangThai = nd.TrangThai
            };

            return View(vm);
        }

        // POST: Admin/NguoiDung/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NguoiDungViewModel vm)
        {
            if (id != vm.MaND) return NotFound();

            if (ModelState.IsValid)
            {
                var nd = await _context.NguoiDungs.FindAsync(id);
                if (nd == null) return NotFound();

                nd.HoTen = vm.HoTen;
                nd.Email = vm.Email;
                nd.SDT = vm.SDT;
                nd.DiaChi = vm.DiaChi;
                nd.LoaiNguoiDung = vm.LoaiNguoiDung;
                nd.TrangThai = vm.TrangThai;

                if (!string.IsNullOrWhiteSpace(vm.MatKhau))
                {
                    nd.MatKhau = BCrypt.Net.BCrypt.HashPassword(vm.MatKhau);
                }

                if (vm.HinhAnhFile != null && vm.HinhAnhFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(nd.HinhAnh))
                    {
                        var oldPath = Path.Combine(_env.WebRootPath, "images", "nguoidung", nd.HinhAnh);
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }

                    string folder = Path.Combine(_env.WebRootPath, "images", "nguoidung");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.HinhAnhFile.FileName);
                    string path = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await vm.HinhAnhFile.CopyToAsync(stream);
                    }
                    nd.HinhAnh = fileName;
                }

                _context.Update(nd);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }
        // POST: Admin/NguoiDung/ToggleStatus/5
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var nd = await _context.NguoiDungs.FindAsync(id);
            if (nd == null) return NotFound();

            nd.TrangThai = (nd.TrangThai == "Hoạt động") ? "Khóa" : "Hoạt động";

            _context.Update(nd);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
