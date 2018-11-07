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
    public class UserController : ControllerBase
    {
        private readonly BlogContext _context;

        public UserController(BlogContext context)
        {
            _context = context;
        }

        [HttpGet]
        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }

        [HttpGet("{id}", Name = "GetUser")]
        public IActionResult GetById(int id)
        {
            var item = _context.Users.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpGet("{id}/blog")]
        public IActionResult GetUsersBlogs(int id)
        {
            var item = _context.Users.Find(id);
            List<BlogItem> blogs = new List<BlogItem>();
            foreach (BlogItem b in _context.BlogItems.Include(op => op.OriginalPoster))
            {
                if (b.OriginalPoster == item)
                {
                    blogs.Add(b);
                }
            }
            if (blogs.Count() == 0)
            {
                return NotFound();
            }
            return Ok(blogs);
        }

        [HttpGet("{id}/blog/{blogid}")]
        public IActionResult GetUsersBlogByID(int id, int blogid)
        {
            // context.Comments.Include(u => u.Poster.Include(p => p.Parent).ToList();
            var item = _context.Users.Find(id);
            List<BlogItem> blogs = new List<BlogItem>();
            foreach (BlogItem b in _context.BlogItems.Include(op => op.OriginalPoster))
            {
                if (b.OriginalPoster == item)
                {
                    blogs.Add(b);
                }
            }

            BlogItem res = blogs.Find(i => i.Id == blogid);
            if (res == null)
            {
                return NotFound();
            }
            return Ok(res);
        }
        // POST api/<controller>
        [HttpPost]
        public IActionResult Create([FromBody] User item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            _context.Users.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute("GetUser", new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] User item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = item.Name;
            user.UserSince = item.UserSince;

            _context.Users.Update(user);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
