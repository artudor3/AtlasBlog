using AtlasBlog.Data;
using AtlasBlog.Models;
using AtlasBlog.Services;
using AtlasBlog.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
var connectionString = ConnectionService.GetConnectionString(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<DataService>();
builder.Services.AddScoped<IImageService, BasicImageService>();
builder.Services.AddTransient<SlugService>();
builder.Services.AddTransient<SearchService>();
builder.Services.AddTransient<IEmailSender, BasicEmailService>();


var app = builder.Build();

//When calling a service from this middleware we need an instance of IServiceScope
var scope = app.Services.CreateScope();
var dataService = scope.ServiceProvider.GetRequiredService<DataService>();
await dataService.SetupDbAsync();

//long code vs tall (36 = 35 & 36)
//await scope.ServiceProvider.GetRequiredService<DataService>().SetupDbAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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

app.MapControllerRoute(
    name: "custom",
    pattern: "PostDetail/{slug}",
    defaults: new { controller = "BlogPosts", action = "Details" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
