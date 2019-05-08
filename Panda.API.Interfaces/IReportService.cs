using Panda.API.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Panda.API.Interfaces
{
    public interface IReportService
    {
        Task<decimal> GetUserBalance(Guid userId);

        Task<List<BurndownValuesViewModel>> GetUserBurndown(Guid userId, int days);
    }
}