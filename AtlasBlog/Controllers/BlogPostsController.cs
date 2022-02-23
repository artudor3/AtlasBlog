#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AtlasBlog.Data;
using AtlasBlog.Models;
using AtlasBlog.Services;
using AtlasBlog.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;

namespace AtlasBlog.Controllers
{
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SlugService _slugService;
        private readonly IImageService _imageService;
        private readonly SearchService _searchService;

        public BlogPostsController(ApplicationDbContext context,
                                   SlugService slugService,
                                   IImageService imageService, 
                                   SearchService searchService)
        {
            _context = context;
            _slugService = slugService;
            _imageService = imageService;
            _searchService = searchService;
        }



        // GET: BlogPosts
        public async Task<IActionResult> BlogChildIndex(int blogId)
        {
            var children = await _context.BlogPosts.Include(b => b.Blog)
                                             .Where(b => b.BlogId == blogId)
                                             .ToPagedListAsync(1,6);
            return View("Index", children);
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BlogPosts.Include(b => b.Blog);
            var pagedPosts = await applicationDbContext.ToPagedListAsync(1,6);
            return View(pagedPosts);
        }

        public async Task<IActionResult> SearchIndex(int? pageNum, string searchTerm)
        {
            pageNum ??= 1;
            var pageSize = 6;

            //Search Service goes here
            var postResults = _searchService.TermSearch(searchTerm);
            var pagedPosts = await postResults.ToPagedListAsync(pageNum, pageSize);

            ViewData["SearchTerm"] = searchTerm;
            return View(pagedPosts);

        }



        // GET: BlogPosts/Details/5
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Blog)
                .Include(c => c.Comments)
                .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (blogPost == null)
            {
                return NotFound();
            }
            

            return View(blogPost);
        }



        // GET: BlogPosts/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "BlogName");
            ViewData["TagIds"] = new MultiSelectList(_context.Tags, "Id", "Text");
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,BlogPostState,Body")] BlogPost blogPost, List<int> tagIds)
        {
            if (ModelState.IsValid)
            {
                var slug = _slugService.UrlFriendly(blogPost.Title, 100);

                //need to make sure the Slug is unique before allowing it to be stored in the DB
                //if unique, it can be used, otherwise throw a custom error
                var isUnique = !_context.BlogPosts.Any(b => b.Slug == slug);

                if (isUnique)
                {
                    blogPost.Slug = slug;
                }
                else
                {
                    ModelState.AddModelError("Title", "This Title cannot be used (duplicate Slug)");
                    ModelState.AddModelError("", "This Title cannot be used");
                    ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "BlogName", blogPost.BlogId);
                    return View(blogPost);
                }

                //Check if tag selections are present
                if(tagIds.Count > 0)
                {
                    var tags = _context.Tags;
                    foreach(var tagId in tagIds)
                    {
                        blogPost.Tags.Add(await tags.FindAsync(tagId));
                    }
                }


                blogPost.Created = DateTime.UtcNow;

                _context.Add(blogPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "BlogName", blogPost.BlogId);
            return View(blogPost);
        }



        // GET: BlogPosts/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var blogPost = await _context.BlogPosts.FindAsync(id);
            var blogPost = await _context.BlogPosts
                                         .Include("Tags")
                                         .FirstOrDefaultAsync(b => b.Id == id);

            var tagIds = await blogPost.Tags.Select(b => b.Id).ToListAsync();

            if (blogPost == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "BlogName", blogPost.BlogId);
            ViewData["TagIds"] = new MultiSelectList(_context.Tags, "Id", "Text", tagIds);
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Slug,IsDeleted,Abstract,BlogPostState,Body,Created")] BlogPost blogPost, List<int> tagIds)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var slug = _slugService.UrlFriendly(blogPost.Title, 100);

                    //need to make sure the Slug is unique before allowing it to be stored in the DB
                    //if unique, it can be used, otherwise throw a custom error
                    if (blogPost.Slug != slug)
                    {
                        var isUnique = !_context.BlogPosts.Any(b => b.Slug == slug);

                        if (isUnique)
                        {
                            blogPost.Slug = slug;
                        }
                        else
                        {
                            ModelState.AddModelError("Title", "This Title cannot be used (duplicate Slug)");
                            ModelState.AddModelError("", "This Title cannot be used");
                            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "BlogName", blogPost.BlogId);
                            ViewData["TagIds"] = new MultiSelectList(_context.Tags, "Id", "Text", tagIds);
                            return View(blogPost);
                        }
                    }

                    //var currentTags = blogPost.Tags.ToList();

                    blogPost.Created = DateTime.SpecifyKind(blogPost.Created, DateTimeKind.Utc);
                    blogPost.Updated = DateTime.UtcNow;

                    _context.Update(blogPost);
                    await _context.SaveChangesAsync();

                    //Tag Management
                    var currentBlogPost = await _context.BlogPosts
                                                        .Include("Tags")
                                                        .FirstOrDefaultAsync(b => b.Id == blogPost.Id);
                    currentBlogPost.Tags.Clear();

                    if (tagIds.Count > 0)
                    {
                        var tags = _context.Tags;
                        foreach (var tagId in tagIds)
                        {
                            blogPost.Tags.Add(await tags.FindAsync(tagId));
                        }
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "BlogName", blogPost.BlogId);
            return View(blogPost);
        }



        // GET: BlogPosts/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Blog)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }
    }
}
