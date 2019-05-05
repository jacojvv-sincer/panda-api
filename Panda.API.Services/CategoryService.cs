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
    public class CategoryService : ICategoryService
    {
        private ApplicationDbContext _context { get; }

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all categories related to transactions of the specified user
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns></returns>
        public async Task<List<Category>> GetCategoriesForUserTransactions(Guid userId)
        {
            return await _context.Transactions
                .Where(t => t.User.Id == userId)
                .Include(t => t.Category)
                .Select(t => t.Category)
                .OrderBy(c => c.Name)
                .Distinct()
                .ToListAsync();
        }
    }
}