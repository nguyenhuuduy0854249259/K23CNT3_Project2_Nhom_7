using Microsoft.EntityFrameworkCore;
using webBanSach.Models;

var builder = WebApplication.CreateBuilder(args);

// Lấy connection string từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DBContext");

// Đăng ký DbContext
builder.Services.AddDbContext<WebBanSachContext>(options =>
    options.UseSqlServer(connectionString));

// --- Đăng ký IHttpContextAccessor để tránh lỗi ---
builder.Services.AddHttpContextAccessor();

// Thêm MVC
builder.Services.AddControllersWithViews();

// --- Cấu hình Session ---
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian sống của session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Middleware xử lý request
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// **Session phải nằm trước Authorization**
app.UseSession();

app.UseAuthorization();

// Route cho các Area
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
