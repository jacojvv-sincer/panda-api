using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Panda.API.Bindings;
using Panda.API.Data;
using Panda.API.Models;

namespace Panda.API.Controllers
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
                .Include(t => t.Category)
                .Include(t => t.Location)
                // .Include(t => t.People)
                // .Include(t => t.Tags)
                .ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Transaction>> Post([FromBody] TransactionBinding transaction)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            Transaction newTransaction = await SetTransactionData(new Transaction(), transaction);
            newTransaction.User = _user;

            _context.Transactions.Add(newTransaction);
            await _context.SaveChangesAsync();
            return Ok(newTransaction);
        }

        [HttpPut]
        public async Task<ActionResult<Transaction>> Update([FromBody] TransactionBinding transaction)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            Transaction transactionToUpdate = await GetUserTransactionById(transaction.Id);
            if (transactionToUpdate == null)
            {
                return NotFound();
            }

            transactionToUpdate = await SetTransactionData(transactionToUpdate, transaction);

            _context.Entry(transactionToUpdate).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(transactionToUpdate);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Transaction transaction = await GetUserTransactionById(id);
            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<Transaction> SetTransactionData(Transaction transaction, TransactionBinding details)
        {
            transaction.Amount = details.Category.Name == "Income" ? details.Amount : -details.Amount;
            transaction.Date = details.Date;
            transaction.IsExtraneous = details.IsExtraneous;
            transaction.Label = details.Label;
            transaction.Notes = details.Notes;

            transaction.Category = details.Category != null ? await _context.Categories.FindAsync(details.Category.Id) : null;
            transaction.Location = details.Location != null ? await _context.Locations.FindAsync(details.Location.Id) : null;

            // Remove people that exist
            if(transaction.TransactionPeople != null){
                _context.RemoveRange(transaction.TransactionPeople);
            }

            if (details.People != null)
            {
                List<int> peopleIds = details.People.Select(p => p.Id).ToList();
                List<Person> people = await _context.People.Where(p => peopleIds.Contains(p.Id)).ToListAsync();
                foreach (Person tag in people)
                {
                    _context.Add(new TransactionPerson { Transaction = transaction, Person = tag });
                }
            }

            // Remove tags that exist
            if(transaction.TransactionTags != null){
                _context.RemoveRange(transaction.TransactionTags);
            }
            
            if (details.Tags != null)
            {
                List<int> tagIds = details.Tags.Select(t => t.Id).ToList();
                List<Tag> tags = await _context.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync();
                foreach (Tag tag in tags)
                {
                    _context.Add(new TransactionTag { Transaction = transaction, Tag = tag });
                }
            }

            return transaction;
        }

        private async Task<Transaction> GetUserTransactionById(int id)
        {
            return await _context.Transactions.Where(t => t.User.Id == _user.Id && t.Id == id).FirstOrDefaultAsync();
        }

    }
}
