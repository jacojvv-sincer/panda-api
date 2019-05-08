using Microsoft.EntityFrameworkCore;
using Panda.API.Data;
using Panda.API.Data.Models;
using Panda.API.Interfaces;
using Panda.API.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Panda.API.Services
{
    public class RelationAnalyticsService : IRelationAnalyticsService
    {
        private ApplicationDbContext _context { get; }

        public RelationAnalyticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns analytics for a specified relation
        /// </summary>
        /// <typeparam name="T">The relation type to retrieve data for</typeparam>
        /// <param name="userId">The id of the user</param>
        /// <param name="relationId">The id of the relation entity</param>
        /// <returns></returns>
        public async Task<RelationAnalyticsViewModel> GetRelationAnalytics<T>(Guid userId, int relationId)
        {
            IQueryable<Transaction> transactions = _context.Transactions.Where(t => t.User.Id == userId);

            Type relationType = typeof(T);
            if (relationType == typeof(Category))
            {
                transactions = transactions.Where(t => t.Category.Id == relationId);
            }
            else if (relationType == typeof(Location))
            {
                transactions = transactions.Where(t => t.Location.Id == relationId);
            }
            else if (relationType == typeof(Person))
            {
                transactions = transactions.Where(t => t.TransactionPeople.Where(p => p.Person.Id == relationId).Any());
            }
            else if (relationType == typeof(Tag))
            {
                transactions = transactions.Where(t => t.TransactionTags.Where(x => x.Tag.Id == relationId).Any());
            }

            int lifetimeCount = await transactions.CountAsync();
            decimal lifetimeTotal = await transactions.Select(t => t.Amount).SumAsync();
            int monthCount = await transactions.Where(t => t.Date > DateTime.Now.AddDays(-30)).CountAsync();
            decimal monthTotal = await transactions.Where(t => t.Date > DateTime.Now.AddDays(-30)).Select(t => t.Amount).SumAsync();

            return new RelationAnalyticsViewModel()
            {
                LifetimeTotalTransactions = lifetimeCount,
                LifetimeSumOfTransactions = lifetimeTotal,
                LifetimeAveragePerTransaction = lifetimeCount > 0 ? lifetimeTotal / lifetimeCount : 0,
                MonthTotalTransactions = monthCount,
                MonthSumOfTransactions = monthTotal,
                MonthAveragePerTransaction = monthCount > 0 ? monthTotal / monthCount : 0
            };
        }
    }
}