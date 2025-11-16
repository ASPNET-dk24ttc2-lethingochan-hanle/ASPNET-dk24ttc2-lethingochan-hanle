using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShopCoffee.Database;
using System.Runtime.Intrinsics.X86;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);    // Thời gian session hết hạn
    options.Cookie.HttpOnly = true;                 // Chỉ server mới truy cập cookie
    options.Cookie.IsEssential = true;              // Bắt buộc, tránh bị chặn bởi GDPR
});

// Thêm dòng này để session hoạt động
builder.Services.AddDistributedMemoryCache();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Customer/Login";
        //truy cập trang có yêu cầu đăng nhập (admin)
        //mà người dùng chưa đăng nhập
        //thì sẽ tự động chuyển sang đường dẫn được khai báo trong LoginPath
        option.AccessDeniedPath = "/AccessDenied";
        // Khi người dùng truy cập trang 
        // mà người dùng đó không được quyền truy cập
        // thì sẽ tự động chuyển sang đường dẫn được khai báo trong AccessDeniedPath
        option.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

builder.Services.AddDbContext<CoffeeShopContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("CoffeeShop"));
});


// Use Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();


// các middleware khác
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=ProductAdmin}/{action=Index}/{id?}");
// nằm trước MapControllerRoute có name là default 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.Run();
