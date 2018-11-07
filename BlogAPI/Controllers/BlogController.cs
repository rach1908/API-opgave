using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly BlogContext _context;

        public BlogController(BlogContext context)
        {
            _context = context;
        }

        [HttpGet]
        public List<BlogItem> GetAll()
        {
            return _context.BlogItems.Include(op => op.OriginalPoster).ToList();
        }

        [HttpGet("{id}", Name = "GetBlog")]
        public IActionResult GetById(int id)
        {
            var item = _context.BlogItems.Where(b => b.Id == id).Include(op => op.OriginalPoster);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpGet("{id}/Comments")]
        public IActionResult GetComments(int id)
        {
            //Include class refrence, to avoid null in api pulls.
            var CommentOp = _context.Comments.Where(b => b.Parent.Id == id).Include(op => op.Poster).Include(v => v.Parent.OriginalPoster);

           
            if (CommentOp.Count() == 0)
            {
                return NotFound();
            }
            return Ok(CommentOp);
        }

        [HttpPost]
        public IActionResult Create([FromBody] BlogItem item)
        {
            var user = _context.Users.Find(item.OriginalPoster.Id);
            if (user == null)
            {
                return BadRequest();
            }
            BlogItem b = new BlogItem();
            b.Title = item.Title;
            b.Subject = item.Subject;
            b.Date = DateTime.Now;
            b.Content = item.Content;
            b.OriginalPoster = user;
            _context.BlogItems.Add(b);
            _context.SaveChanges();

            return CreatedAtRoute("GetBlog", new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] BlogItem item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }
            var blog = _context.BlogItems.Find(id);
            if (blog == null)
            {
                return NotFound();
            }

            blog.Title = item.Title;
            blog.Subject = item.Subject;
            blog.Content = item.Content;
            blog.Date = item.Date;
            blog.OriginalPoster = item.OriginalPoster;

            _context.BlogItems.Update(blog);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var blog = _context.BlogItems.Find(id);
            if (blog == null)
            {
                return NotFound();
            }

            _context.BlogItems.Remove(blog);
            _context.SaveChanges();
            return NoContent();
        }

    }
}
