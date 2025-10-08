using Microsoft.EntityFrameworkCore;
using webBanSach.Models;

var builder = WebApplication.CreateBuilder(args);

// ===================== CẤU HÌNH DỊCH VỤ =====================

// 1. Kết nối database
var connectionString = builder.Configuration.GetConnectionString("DBContext");
builder.Services.AddDbContext<WebBanSachContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Cho phép dùng HttpContext trong controller / view
builder.Services.AddHttpContextAccessor();

// 3. Thêm MVC
builder.Services.AddControllersWithViews();

// 4. Cấu hình SESSION
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // thời gian session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ===================== MIDDLEWARE =====================

// Xử lý lỗi và HSTS
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// HTTPS và static files
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Phải đặt UseSession TRƯỚC middleware kiểm tra admin
app.UseSession();

// ===================== MIDDLEWARE KIỂM TRA ADMIN LOGIN =====================
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower();

    // Nếu truy cập vào /admin mà chưa đăng nhập admin thì redirect về login
    if (path != null && path.StartsWith("/admin"))
    {
        var adminId = context.Session.GetInt32("AdminId"); // phải trùng với key session khi login

        // Bỏ qua trang login admin để tránh redirect vòng lặp
        if (!path.Contains("/adminaccount/login") && adminId == null)
        {
            context.Response.Redirect("/Admin/AdminAccount/Login");
            return;
        }
    }

    await next();
});

app.UseAuthorization();

// ===================== ROUTE =====================

// Route cho Area Admin
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
