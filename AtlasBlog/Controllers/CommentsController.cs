﻿#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AtlasBlog.Data;
using AtlasBlog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace AtlasBlog.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;

        public CommentsController(ApplicationDbContext context, UserManager<BlogUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogPostId,CommentBody")] Comment comment, string slug)
        {
            if (ModelState.IsValid)
            {
                comment.AuthorId = _userManager.GetUserId(User);
                comment.CreatedDate = DateTime.UtcNow;
                
                _context.Add(comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details","BlogPosts",new { slug },"CommentSection");
        }

        // POST: Comments/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CommentBody")] Comment comment, string slug)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var commentSnapShot = await _context.Comments.FindAsync(comment.Id);
                    if (commentSnapShot == null)
                    {
                        return NotFound();
                    }
                    commentSnapShot.CommentBody = comment.CommentBody;
                    commentSnapShot.UpdatedDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "BlogPosts", new { slug }, "CommentSection");
            }


            //ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", comment.AuthorId);
            //ViewData["BlogPostId"] = new SelectList(_context.BlogPosts, "Id", "Abstract", comment.BlogPostId);
            return View(comment);
        }
                
        // POST: Comments/Moderate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Moderate(int id, [Bind("Id,ModeratedBody,ModerationReason")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            Comment commentSnapShot = new();
            try
            {
                //var commentSnapShot = await _context.Comments.FindAsync(comment.Id);
                commentSnapShot = await _context.Comments
                                                    .Include(c => c.BlogPost)
                                                    .FirstOrDefaultAsync(c => c.Id == comment.Id);

                if (commentSnapShot == null)
                {
                    return NotFound();
                }

                commentSnapShot.ModeratedDate = DateTime.UtcNow;
                commentSnapShot.ModeratedBody = comment.ModeratedBody;
                commentSnapShot.ModerationReason = comment.ModerationReason;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(comment.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("Details", "BlogPosts", new { slug = commentSnapShot.BlogPost.Slug }, "CommentSection");
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments
                                        .Include(c => c.BlogPost)
                                        .FirstOrDefaultAsync(c => c.Id == id);

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "BlogPosts", new { slug = comment.BlogPost.Slug }, "CommentSection");
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
