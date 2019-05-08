using Panda.API.ViewModels;
using System;
using System.Threading.Tasks;

namespace Panda.API.Interfaces
{
    public interface IRelationAnalyticsService
    {
        Task<RelationAnalyticsViewModel> GetRelationAnalytics<T>(Guid userId, int relationId);
    }
}