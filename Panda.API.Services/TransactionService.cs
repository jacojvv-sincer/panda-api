using Microsoft.EntityFrameworkCore;
using Panda.API.Data;
using Panda.API.Data.Models;
using Panda.API.Exceptions;
using Panda.API.Interfaces;
using Panda.API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panda.API.Services
{
    public class TransactionService : ITransactionService
    {
        private ApplicationDbContext _context { get; }

        /// <summary>
        /// This service is used to interact with transactions in the database
        /// </summary>
        /// <param name="context"></param>
        public TransactionService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a count of transactions for a user
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns></returns>
        public async Task<int> GetCountOfUserTransactions(Guid userId)
        {
            return await _context.Transactions.Where(t => t.User.Id == userId).CountAsync();
        }

        /// <summary>
        /// Returns transactions ordered by date descending for the user
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <param name="page">The page to retrieve</param>
        /// <param name="perPage">The total items to return per page</param>
        /// <returns></returns>
        public async Task<List<Transaction>> GetUserTransactions(Guid userId, int page = 1, int perPage = 30)
        {
            IQueryable<Transaction> baseQuery = _context.Transactions.Where(t => t.User.Id == userId);
            IQueryable<Transaction> query = baseQuery.OrderByDescending(t => t.Date)
                                                     .ThenByDescending(t => t.Id)
                                                     .Include(t => t.Category)
                                                     .Include(t => t.Location)
                                                     .Include(t => t.TransactionTags)
                                                     .ThenInclude(x => x.Tag)
                                                     .Include(t => t.TransactionPeople)
                                                     .ThenInclude(p => p.Person)
                                                     .Skip((page - 1) * perPage)
                                                     .Take(perPage);

            // more query logic to follow - hence UnixTimeHelper.cs

            return await query.ToListAsync();
        }

        /// <summary>
        /// Retrieves the transaction of a user by id
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <param name="id">The id of the transaction</param>
        /// <returns></returns>
        public async Task<Transaction> GetUserTransactionById(Guid userId, int id)
        {
            return await _context.Transactions.Where(t => t.User.Id == userId && t.Id == id)
                                              .Include(t => t.Category)
                                              .Include(t => t.Location)
                                              .Include(t => t.TransactionTags)
                                              .ThenInclude(x => x.Tag)
                                              .Include(t => t.TransactionPeople)
                                              .ThenInclude(p => p.Person)
                                              .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Saves a new transaction for the user
        /// </summary>
        /// <param name="user">The user to save the transaction for</param>
        /// <param name="model">The transaction viewmodel</param>
        /// <returns></returns>
        public async Task<Transaction> SaveTransaction(User user, AddTransactionViewModel model)
        {
            Transaction transaction = await SetTransactionData(new Transaction(), model);
            transaction.User = user;

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        /// <summary>
        /// Updates a transaction of the user
        /// </summary>
        /// <param name="user">The user to update the transaction for</param>
        /// <param name="model">The transaction viewmodel</param>
        /// <returns></returns>
        public async Task<Transaction> UpdateTransaction(User user, AddTransactionViewModel model)
        {
            Transaction transactionToUpdate = await GetUserTransactionById(user.Id, model.Id);
            if (transactionToUpdate == null)
            {
                return null;
            }

            transactionToUpdate = await SetTransactionData(transactionToUpdate, model);

            _context.Entry(transactionToUpdate).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return transactionToUpdate;
        }

        /// <summary>
        /// Deletes a transaction
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="transactionId"></param>
        /// <returns>True if the transaction is deleted</returns>
        public async Task<bool> DeleteTransaction(Guid userId, int transactionId)
        {
            Transaction transaction = await GetUserTransactionById(userId, transactionId);
            if (transaction == null)
            {
                throw new TransactionNotFoundException();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<Transaction> SetTransactionData(Transaction transaction, AddTransactionViewModel details)
        {
            transaction.Amount = details.Category.Name == "Income" ? details.Amount : -details.Amount;
            transaction.Date = details.Date;
            transaction.IsExtraneous = details.IsExtraneous;
            transaction.Label = details.Label;
            transaction.Notes = details.Notes;

            transaction.Category = details.Category != null ? await _context.Categories.FindAsync(details.Category.Id) : null;
            transaction.Location = details.Location != null ? await _context.Locations.FindAsync(details.Location.Id) : null;

            // Remove people that exist
            if (transaction.TransactionPeople != null)
            {
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
            if (transaction.TransactionTags != null)
            {
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
    }
}