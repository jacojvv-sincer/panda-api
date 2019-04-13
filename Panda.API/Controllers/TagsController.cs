using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Panda.API.Data;
using Panda.API.Models;

namespace Panda.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private ApplicationDbContext _context;
        private User _user;

        public TagsController(ApplicationDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _user = (User)http.HttpContext.Items["ApplicationUser"];
        }

        [HttpGet]
        public async Task<ActionResult<List<Tag>>> Get()
        {
            List<Tag> tags = await _context.Transactions
                .Where(t => t.User.Id == _user.Id)
                .Include(t => t.TransactionTags)
                .ThenInclude(t => t.Tag)
                .Select(t => t.TransactionTags.Select(x => x.Tag)) // get the tags
                .SelectMany(t => t) // flatten
                .OrderBy(t => t.Name)
                .Distinct()
                .ToListAsync();

            return Ok(tags);
        }

        [HttpPost]
        public async Task<ActionResult<Tag>> Post([FromBody] Tag tag)
        {
            if(!ModelState.IsValid){
                return UnprocessableEntity(ModelState);
            }

            var existingTag = await _context.Tags.Where(c => c.Name == tag.Name).FirstOrDefaultAsync();
            if(existingTag != null){
                return Ok(existingTag);
            }
            
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return Ok(tag);
        }
    }
}
