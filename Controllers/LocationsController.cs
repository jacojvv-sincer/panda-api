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
    public class LocationsController : ControllerBase
    {
        private ApplicationDbContext _context;
        private User _user;

        public LocationsController(ApplicationDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _user = (User)http.HttpContext.Items["ApplicationUser"];
        }

        [HttpPost]
        public async Task<ActionResult<Location>> Post([FromBody] Location location)
        {
            if(!ModelState.IsValid){
                return UnprocessableEntity(ModelState);
            }

            var existingLocation = await _context.Locations.Where(c => c.Name == location.Name).FirstOrDefaultAsync();
            if(existingLocation != null){
                return Ok(existingLocation);
            }
            
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            return Ok(location);
        }
    }
}
