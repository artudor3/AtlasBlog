using AtlasBlog.Data;
using AtlasBlog.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using X.PagedList;

namespace AtlasBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(int? pageNum)
        {
            pageNum = pageNum ?? 1;
            pageNum ??= 1;

            //var blogs = await _context.Blogs.OrderByDescending(b => b.Created).ToListAsync();
            //var blogs = _context.Blogs.ToPagedList((int)pageNum, 5);
            var blogs = await _context.Blogs.OrderByDescending(b => b.Created)
                                            .ToPagedListAsync(pageNum, 5);
            return View(blogs);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}