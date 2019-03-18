using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyManagerApi.Data;
using MoneyManagerApi.Models;

namespace MoneyManagerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemarcationsController : ControllerBase
    {
        private ApplicationDbContext _context;
        private User _user;

        public DemarcationsController(ApplicationDbContext context)
        {
            _context = context;
            _user = (User)HttpContext.Items["ApplicationUser"];
        }

        [HttpGet]
        public async Task<List<Demarcation>> Get()
        {
            return await _context.Demarcations.Where(d => d.User.Id == _user.Id).ToListAsync();
        }
    }
}
