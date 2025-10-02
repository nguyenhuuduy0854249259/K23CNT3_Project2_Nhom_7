using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webBanSach.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace webBanSach.Controllers
{
    public class DonHangController : Controller
    {
        private readonly WebBanSachContext _context;

        public DonHangController(WebBanSachContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách đơn hàng đã mua
        // Hiển thị danh sách đơn hàng đã mua
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("MaND");
            if (userId == null) return RedirectToAction("Login", "Account");

            var donHangs = await _context.DonHangs
                .Where(d => d.MaND == userId.Value)
                .OrderByDescending(d => d.NgayDat) // mới nhất lên đầu
                .ToListAsync();

            return View(donHangs);
        }


        public async Task<IActionResult> Details(int id)
        {
            var donHang = await _context.DonHangs
                .Include(d => d.MaNDNavigation)
                .Include(d => d.CT_DonHangs)
                    .ThenInclude(ct => ct.MaSachNavigation)
                .FirstOrDefaultAsync(d => d.MaDH == id);

            if (donHang == null) return NotFound();

            return View(donHang);
        }

    }
}
