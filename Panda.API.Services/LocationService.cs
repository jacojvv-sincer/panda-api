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
    public class LocationService : ILocationService
    {
        private ApplicationDbContext _context { get; }

        public LocationService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all locations related to transactions of the specified user
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns></returns>
        public async Task<List<Location>> GetLocationsForUserTransactions(Guid userId)
        {
            return await _context.Transactions
                .Where(t => t.User.Id == userId)
                .Include(t => t.Location)
                .Select(t => t.Location)
                .OrderBy(c => c.Name)
                .Distinct()
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new location and returns
        ///
        /// Will return a previously existing locatio  if the category already exists
        /// </summary>
        /// <param name="location">The category to add</param>
        /// <returns></returns>
        public async Task<Location> CreateLocation(Location location)
        {
            var existingLocation = await _context.Locations.Where(c => c.Name == location.Name).FirstOrDefaultAsync();
            if (existingLocation != null)
                return existingLocation;

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            return location;
        }
    }
}