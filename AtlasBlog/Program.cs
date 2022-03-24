using AtlasBlog.Data;
using AtlasBlog.Models;
using AtlasBlog.Services;
using AtlasBlog.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
var connectionString = ConnectionService.GetConnectionString(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

//Adding in a CORS policy to avoid getting denied from my portfolio
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", builder => 
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});




builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

//Register the Swagger Service
builder.Services.AddSwaggerGen(s =>
    {
        OpenApiInfo openApiInfo = new()
        {
            Title = "Atlas Blog API",
            Version = "v1",
            Description = "Candidate API for the Atlas Blog",
            Contact = new()
            {
                Name = "Andrew R Tudor",
                Url = new("https://andrew-tudor.netlify.app/")
            },
            License = new()
            {
                Name = "Api License",
                Url = new("https://andrew-tudor.netlify.app/")
            }
        };
        s.SwaggerDoc(openApiInfo.Version, openApiInfo);

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        s.IncludeXmlComments(xmlPath);
    }
    
    );

builder.Services.AddTransient<DataService>();
builder.Services.AddScoped<IImageService, BasicImageService>();
builder.Services.AddTransient<IEmailSender, BasicEmailService>();
builder.Services.AddTransient<SlugService>();
builder.Services.AddTransient<SearchService>();


var app = builder.Build();

//When calling a service from this middleware we need an instance of IServiceScope
var scope = app.Services.CreateScope();
var dataService = scope.ServiceProvider.GetRequiredService<DataService>();
await dataService.SetupDbAsync();

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

app.UseCors("DefaultCorsPolicy");


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


//Call on Swagger service
app.UseSwagger();
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Atlas Blog API");
    s.InjectStylesheet("/css/swaggerUI.css");
    s.InjectJavascript("/js/swaggerUI.js");

    if (!app.Environment.IsDevelopment())
    {
        s.RoutePrefix = "";
    }
});



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
