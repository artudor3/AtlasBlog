#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AtlasBlog.Data;
using AtlasBlog.Models;
using AtlasBlog.Enums;

namespace AtlasBlog.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BlogPostsApiController(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Return the specified number of latest Posts
        /// </summary>
        /// <remarks>
        /// I can add additional descriptive data here
        /// </remarks>
        /// <param name="num">integer count of records</param>
        /// <returns>
        /// Returns a list of Blog Posts
        /// </returns>
        [HttpGet("GetTopXPosts/{num:int}")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetTopXPosts(int num)
        {
            //Return the top x latest production ready postsd
            // & the latest posts
            var posts = await _context.BlogPosts
                                .Where(b => !b.IsDeleted && 
                                       b.BlogPostState == BlogPostState.ProductionReady)
                                .OrderByDescending(b => b.Created)
                                .Take(num)
                                .ToListAsync();
            return posts;
        
        }

        // GET: api/BlogPostsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogPosts()
        {
            //Returns All Blog Posts
            return await _context.BlogPosts.ToListAsync();
        }

        //// GET: api/BlogPostsApi/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<BlogPost>> GetBlogPost(int id)
        //{
        //    var blogPost = await _context.BlogPosts.FindAsync(id);

        //    if (blogPost == null)
        //    {
        //        return NotFound();
        //    }

        //    return blogPost;
        //}

        //// PUT: api/BlogPostsApi/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutBlogPost(int id, BlogPost blogPost)
        //{
        //    if (id != blogPost.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(blogPost).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!BlogPostExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/BlogPostsApi
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<BlogPost>> PostBlogPost(BlogPost blogPost)
        //{
        //    _context.BlogPosts.Add(blogPost);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetBlogPost", new { id = blogPost.Id }, blogPost);
        //}

        //// DELETE: api/BlogPostsApi/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteBlogPost(int id)
        //{
        //    var blogPost = await _context.BlogPosts.FindAsync(id);
        //    if (blogPost == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.BlogPosts.Remove(blogPost);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool BlogPostExists(int id)
        //{
        //    return _context.BlogPosts.Any(e => e.Id == id);
        //}
    }
}
