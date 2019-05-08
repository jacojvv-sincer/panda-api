using Microsoft.EntityFrameworkCore;
using Panda.API.Data;
using Panda.API.Data.Models;
using Panda.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panda.API.Services
{
    public class PersonService : IPersonService
    {
        private ApplicationDbContext _context { get; }

        public PersonService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all people related to transactions of the specified user
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns></returns>
        public async Task<List<Person>> GetPeopleForUserTransactions(Guid userId)
        {
            return await _context.Transactions
                .Where(t => t.User.Id == userId)
                .Include(t => t.TransactionPeople)
                .Select(t => t.TransactionPeople.Select(x => x.Person))
                .SelectMany(p => p) // flatten
                .OrderBy(p => p.Name)
                .Distinct()
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new person and returns
        ///
        /// Will return a previously existing person if the person already exists
        /// </summary>
        /// <param name="person">The person to add</param>
        /// <returns></returns>
        public async Task<Person> CreatePerson(Person person)
        {
            var existingPerson = await _context.People.Where(c => c.Name == person.Name).FirstOrDefaultAsync();
            if (existingPerson != null)
                return existingPerson;

            _context.People.Add(person);
            await _context.SaveChangesAsync();
            return person;
        }
    }
}