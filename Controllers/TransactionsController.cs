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
    public class TransactionsController : ControllerBase
    {
        private ApplicationDbContext _context;
        private User _user;

        public TransactionsController(ApplicationDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _user = (User)http.HttpContext.Items["ApplicationUser"];
        }

        [HttpGet]
        public async Task<ActionResult<List<Transaction>>> Get()
        {
            return Ok(await _context.Transactions
                .Where(t => t.User.Id == _user.Id)
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.Id)
                .Include(t => t.Category )
                .Include(t => t.Location)
                .Include(t => t.People)
                .Include(t => t.Tags)
                .ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Transaction>> Post([FromBody] Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var newTransaction = new Transaction();
            newTransaction.Amount = transaction.Category.Name == "Income" ? transaction.Amount : -transaction.Amount;
            newTransaction.Date = transaction.Date;
            newTransaction.IsExtraneous = transaction.IsExtraneous;
            newTransaction.Label = transaction.Label;
            newTransaction.Notes = transaction.Notes;

            if (transaction.Category != null)
            {
                newTransaction.Category = await _context.Categories.FindAsync(transaction.Category.Id);
            }
            if (transaction.Location != null)
            {
                newTransaction.Location = await _context.Locations.FindAsync(transaction.Location.Id);
            }
            if (transaction.People != null)
            {
                var ids = transaction.People.Select(p => p.Id).ToArray();
                newTransaction.People = await _context.People.Where(p => ids.Contains(p.Id)).ToListAsync();
            }
            if (transaction.Tags != null)
            {
                var ids = transaction.Tags.Select(t => t.Id).ToList();
                newTransaction.Tags = await _context.Tags.Where(t => ids.Contains(t.Id)).ToListAsync();
            }

            newTransaction.User = _user;
            _context.Transactions.Add(newTransaction);
            await _context.SaveChangesAsync();
            return Ok(newTransaction);
        }

    }
}
