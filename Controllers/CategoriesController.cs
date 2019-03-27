using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyManagerApi.Data;
using MoneyManagerApi.Models;

namespace MoneyManagerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private ApplicationDbContext _context;
        private User _user;

        public CategoriesController(ApplicationDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _user = (User)http.HttpContext.Items["ApplicationUser"];
        }

        // [HttpGet]
        // public async Task<ActionResult<List<Transaction>>> Get()
        // {
        //     return Ok(await _context.Transactions.Where(d => d.User.Id == _user.Id).OrderByDescending(d => d.Date).ToListAsync());
        // }

        // [HttpGet]
        // [Route("latest")]
        // public async Task<ActionResult<List<Demarcation>>> GetLatest()
        // {
        //     return Ok(await _context.Demarcations.Include(d => d.Buckets).ThenInclude(b => b.).Where(d => d.User.Id == _user.Id).OrderByDescending(d => d.StartDate).FirstAsync());
        // }

        [HttpPost]
        public async Task<ActionResult<Category>> Post([FromBody] Category category)
        {
            if(!ModelState.IsValid){
                return UnprocessableEntity(ModelState);
            }

            var existingCategory = await _context.Categories.Where(c => c.Name == category.Name).FirstOrDefaultAsync();
            if(existingCategory != null){
                return Ok(existingCategory);
            }
            
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }
    }
}
