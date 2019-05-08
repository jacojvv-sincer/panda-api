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
    public class TagService : ITagService
    {
        private ApplicationDbContext _context { get; }

        public TagService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all tags related to transactions of the specified user
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns></returns>
        public async Task<List<Tag>> GetTagsForUserTransactions(Guid userId)
        {
            return await _context.Transactions
                .Where(t => t.User.Id == userId)
                .Include(t => t.TransactionTags)
                .Select(t => t.TransactionTags.Select(x => x.Tag))
                .SelectMany(p => p) // flatten
                .OrderBy(p => p.Name)
                .Distinct()
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new tag and returns
        ///
        /// Will return a previously existing tag if the tag already exists
        /// </summary>
        /// <param name="tag">The person to add</param>
        /// <returns></returns>
        public async Task<Tag> CreateTag(Tag tag)
        {
            var existingTag = await _context.Tags.Where(c => c.Name == tag.Name).FirstOrDefaultAsync();
            if (existingTag != null)
                return existingTag;

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }
    }
}