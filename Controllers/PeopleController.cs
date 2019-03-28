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
    public class PeopleController : ControllerBase
    {
        private ApplicationDbContext _context;
        private User _user;

        public PeopleController(ApplicationDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _user = (User)http.HttpContext.Items["ApplicationUser"];
        }

        [HttpPost]
        public async Task<ActionResult<Person>> Post([FromBody] Person person)
        {
            if(!ModelState.IsValid){
                return UnprocessableEntity(ModelState);
            }

            var existingPerson = await _context.People.Where(c => c.Name == person.Name).FirstOrDefaultAsync();
            if(existingPerson != null){
                return Ok(existingPerson);
            }
            
            _context.People.Add(person);
            await _context.SaveChangesAsync();
            return Ok(person);
        }
    }
}
