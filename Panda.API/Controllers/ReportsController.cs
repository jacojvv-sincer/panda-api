using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Panda.API.Data;
using Panda.API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panda.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private ApplicationDbContext _context;
        private User _user;

        public ReportsController(ApplicationDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _user = (User)http.HttpContext.Items["ApplicationUser"];
        }

        [HttpGet]
        [Route("Balance")]
        public ActionResult<decimal> Balance()
        {
            return Ok(_context.Transactions.Where(t => t.User.Id == _user.Id).Sum(t => t.Amount));
        }

        // todo : move somewhere else
        public class BurndownValues
        {
            public DateTime Date { get; set; }
            public Decimal Total { get; set; }
        }

        [HttpGet]
        [Route("Burndown")]
        public async Task<ActionResult> Burndown([FromQuery] int days = 30)
        {
            days = days > 730 ? 730 : days;
            days = days < 0 ? 30 : days;

            DateTime date = DateTime.Today.AddDays(-days);
            var balance = _context.Transactions.Where(t => t.User.Id == _user.Id && t.Date <= date).Sum(t => t.Amount);
            var transactions = await _context.Transactions.Where(t => t.User.Id == _user.Id && t.Date > date).ToListAsync();

            List<BurndownValues> values = new List<BurndownValues>();

            for (int i = 0; i < days; i++)
            {
                date = date.AddDays(1);
                balance = balance + transactions.Where(t => t.Date == date).Sum(t => t.Amount);
                values.Add(new BurndownValues
                {
                    Date = date,
                    Total = balance
                });
            }

            return Ok(values);
        }
    }
}