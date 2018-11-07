using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly BlogContext _context;

        public CommentController(BlogContext context)
        {
            _context = context;
        }

        [HttpGet]
        public List<Comment> GetAll()
        {
            var res  =_context.Comments
                            .Include(u => u.Poster)
                            .Include(p => p.Parent).ToList();
            return res;
        }

        [HttpGet("{id}", Name = "GetComment")]
        public IActionResult GetById(int id)
        {
            var item = _context.Comments.Where(b => b.Id == id).Include(op => op.Poster);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Comment item)
        {
            User user = _context.Users.Find(item.Poster.Id);
            var parent = _context.BlogItems.Find(item.Parent.Id);
            if (user == null || parent == null)
            {
                return BadRequest();
            }
            Comment c = new Comment();
            c.Parent = parent;
            c.Poster = user;
            c.Title = item.Title;
            c.Content = item.Content;
            c.Posted = DateTime.Now;
            _context.Comments.Add(c);
            _context.SaveChanges();

            return CreatedAtRoute("GetComment", new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Comment item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            var comment = _context.Comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }

            comment.Poster = item.Poster;
            comment.Posted = item.Posted;
            comment.Title = item.Title;
            comment.Content = item.Content;
            comment.Parent = comment.Parent;

            _context.Comments.Update(comment);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var comment = _context.Comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
