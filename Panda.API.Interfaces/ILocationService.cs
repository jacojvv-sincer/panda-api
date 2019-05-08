using Panda.API.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Panda.API.Interfaces
{
    public interface ILocationService
    {
        Task<List<Location>> GetLocationsForUserTransactions(Guid userId);

        Task<Location> CreateLocation(Location category);
    }
}