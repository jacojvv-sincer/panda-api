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
        public ActionResult<decimal> Balance(){
            return Ok(_context.Transactions.Where(t => t.User.Id == _user.Id).Sum(t => t.Amount));
        }

        // todo : move somewhere else
        public class BurndownValues {
            public DateTime Date { get; set; }
            public Decimal Total { get; set; }
        }



        [HttpGet]
        [Route("Burndown")]
        public async Task<ActionResult> Burndown(){
            DateTime date = DateTime.Today.AddDays(-30);
            var balance = _context.Transactions.Where(t => t.User.Id == _user.Id && t.Date <= date).Sum(t => t.Amount);
            var transactions = await _context.Transactions.Where(t => t.User.Id == _user.Id && t.Date > date).ToListAsync();

            List<BurndownValues> values = new List<BurndownValues>();

            for (int i = 0; i < 30; i++)
            {
                date = date.AddDays(1);
                balance = balance + transactions.Where(t => t.Date == date).Sum(t => t.Amount);
                values.Add(new BurndownValues{
                    Date = date,
                    Total = balance 
                });
            }


            return Ok(values);
        }

    }
}
