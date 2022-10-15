using App.Business.Abstract;
using App.Business.Concrete;
using App.DataAccess.Abstract;
using App.DataAccess.Concrete.EfEntityFramework;
using App.MvcWebUI.Entities;
using App.MvcWebUI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IProductService, ProductManager>();
builder.Services.AddScoped<IProductDal, EfProductDal>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<ICategoryDal, EfCategoryDal>();
builder.Services.AddSingleton<ICartSessionService, CartSessionService>();
builder.Services.AddSingleton<ICartService, CartService>();

builder.Services.AddDbContext<CustomIdentityDbContext>
    (options => options.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Northwind;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"));

builder.Services.AddIdentity<CustomIdentityUser, CustomIdentityRole>()
    .AddEntityFrameworkStores<CustomIdentityDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();

// Add services to the container.
builder.Services.AddControllersWithViews();


//add auth policy for admin
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Edit", policy => policy.RequireClaim("Edit", "true"));
    options.AddPolicy("Add", policy => policy.RequireClaim("Add", "true"));
    options.AddPolicy("Delete", policy => policy.RequireClaim("Delete", "true"));
    options.AddPolicy("CreateRole", policy => policy.RequireClaim("CreateRole", "true"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();



app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
