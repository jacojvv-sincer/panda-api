using Microsoft.EntityFrameworkCore;
using Panda.API.Data;
using Panda.API.Interfaces;
using Panda.API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panda.API.Services
{
    public class ReportService : IReportService
    {
        private ApplicationDbContext _context { get; }

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return the balance of the specified user
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns></returns>
        public async Task<decimal> GetUserBalance(Guid userId)
        {
            return await _context.Transactions.Where(t => t.User.Id == userId).SumAsync(t => t.Amount);
        }

        /// <summary>
        /// Retrieves the burndown of the specified users funds for the specified amount of days
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <param name="days">The amount of days to return a burndown for</param>
        /// <returns></returns>
        public async Task<List<BurndownValuesViewModel>> GetUserBurndown(Guid userId, int days)
        {
            days = days > 730 ? 730 : days;
            days = days < 0 ? 30 : days;

            DateTime date = DateTime.Today.AddDays(-days);
            var balance = _context.Transactions.Where(t => t.User.Id == userId && t.Date <= date).Sum(t => t.Amount);
            var transactions = await _context.Transactions.Where(t => t.User.Id == userId && t.Date > date).ToListAsync();

            List<BurndownValuesViewModel> values = new List<BurndownValuesViewModel>();

            for (int i = 0; i < days; i++)
            {
                date = date.AddDays(1);
                balance = balance + transactions.Where(t => t.Date == date).Sum(t => t.Amount);
                values.Add(new BurndownValuesViewModel
                {
                    Date = date,
                    Total = balance
                });
            }

            return values;
        }
    }
}