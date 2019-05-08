using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panda.API.Data;
using Panda.API.Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Panda.API.Services.Tests
{
    [TestClass]
    public class LocationServiceTests
    {
        private ApplicationDbContext _applicationDbContext { get; set; }
        private LocationService _locationService { get; set; }

        private User user;
        private static Guid userId = Guid.NewGuid();
        private Transaction userTransaction;
        private Location capeTownLocation;

        [TestInitialize]
        public void Setup()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("testing_database");
            _applicationDbContext = new ApplicationDbContext(optionsBuilder.Options);

            // Setup groceries category
            capeTownLocation = new Location() { Name = "Cape Town" };
            _applicationDbContext.Locations.Add(capeTownLocation);

            // user setup
            user = new User()
            {
                Id = userId
            };
            _applicationDbContext.Users.Add(user);
            userTransaction = new Transaction()
            {
                User = user,
                Amount = -12,
                Location = capeTownLocation
            };
            _applicationDbContext.Transactions.Add(userTransaction);

            //Save changes
            _applicationDbContext.SaveChanges();

            _locationService = new LocationService(_applicationDbContext);
        }

        [TestCleanup]
        public void TearDown()
        {
            _applicationDbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetLocationsForUserTransactions_Should_ReturnAllLocationsForUserTransactions()
        {
            var locations = await _locationService.GetLocationsForUserTransactions(userId);
            Assert.AreEqual(1, locations.Count);
            Assert.AreEqual("Cape Town", locations.First().Name);
        }

        [TestMethod]
        public async Task CreateLocation_Should_PersistTheLocationToTheDatabase()
        {
            var location = await _locationService.CreateLocation(new Location() { Name = "New York" });
            Assert.AreEqual(1, _applicationDbContext.Locations.Where(c => c.Name == "New York").Count());
        }

        [TestMethod]
        public async Task CreateLocation_Should_ReturnExistingLocationsUponSameCreation()
        {
            var category = await _locationService.CreateLocation(capeTownLocation);
            Assert.AreEqual(category.Id, capeTownLocation.Id);
        }
    }
}