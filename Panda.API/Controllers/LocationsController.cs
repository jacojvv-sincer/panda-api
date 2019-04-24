using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Panda.API.Data;
using Panda.API.Models;
using Panda.API.ViewModels;

namespace Panda.API.Controllers
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

        [HttpGet]
        public async Task<ActionResult<List<Location>>> Get()
        {
            return Ok(await _context.Transactions
                .Where(t => t.User.Id == _user.Id)
                .Include(t => t.Location)
                .Select(t => t.Location)
                .OrderBy(l => l.Name)
                .Distinct()
                .ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Location>> Post([FromBody] Location location)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var existingLocation = await _context.Locations.Where(c => c.Name == location.Name).FirstOrDefaultAsync();
            if (existingLocation != null)
            {
                return Ok(existingLocation);
            }

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            return Ok(location);
        }

        [HttpGet]
        [Route("{id}/Analytics")]
        public async Task<RelationAnalyticsViewModel> Analytics(int id)
        {
            IQueryable<Transaction> transactions = _context.Transactions.Where(t => t.User.Id == _user.Id && t.Location.Id == id);

            int lifetimeCount = await transactions.CountAsync();
            decimal lifetimeTotal = await transactions.Select(t => t.Amount).SumAsync();
            int monthCount = await transactions.Where(t => t.Date > DateTime.Now.AddDays(-30)).CountAsync();
            decimal monthTotal = await transactions.Where(t => t.Date > DateTime.Now.AddDays(-30)).Select(t => t.Amount).SumAsync();

            return new RelationAnalyticsViewModel()
            {
                LifetimeTotalTransactions = lifetimeCount,
                LifetimeSumOfTransactions = lifetimeTotal,
                LifetimeAveragePerTransaction = lifetimeTotal / lifetimeCount,
                MonthTotalTransactions = monthCount,
                MonthSumOfTransactions = monthTotal,
                MonthAveragePerTransaction = monthTotal / monthCount
            };
        }
    }
}
