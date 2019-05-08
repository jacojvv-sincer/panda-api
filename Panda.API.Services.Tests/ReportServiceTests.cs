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
    public class ReportServiceTests
    {
        private ApplicationDbContext _applicationDbContext { get; set; }
        private ReportService _reportService { get; set; }

        private User user;
        private static Guid userId = Guid.NewGuid();
        private Transaction userTransaction;
        private static Guid userId2 = Guid.NewGuid();

        [TestInitialize]
        public void Setup()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("testing_database");
            _applicationDbContext = new ApplicationDbContext(optionsBuilder.Options);

            // user setup
            user = new User()
            {
                Id = userId
            };
            _applicationDbContext.Users.Add(user);
            userTransaction = new Transaction()
            {
                User = user,
                Amount = 200,
                Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-100)
            };
            _applicationDbContext.Transactions.Add(userTransaction);

            // second user setup
            User user3 = new User()
            {
                Id = userId2
            };
            _applicationDbContext.Users.Add(user3);
            for (int i = 0; i < 100; i++)
            {
                _applicationDbContext.Transactions.Add(new Transaction()
                {
                    User = user3,
                    Amount = 3,
                    Date = DateTime.Now
                });
            }

            //Save changes
            _applicationDbContext.SaveChanges();

            _reportService = new ReportService(_applicationDbContext);
        }

        [TestCleanup]
        public void TearDown()
        {
            _applicationDbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetUserBalance_Should_ReturnTheCorrectValue()
        {
            var userBalance = await _reportService.GetUserBalance(userId);
            Assert.AreEqual(200, userBalance);

            var user2Balance = await _reportService.GetUserBalance(userId2);
            Assert.AreEqual(300, user2Balance);
        }

        [TestMethod]
        public async Task GetUserBurndown_Should_ReturnTheCorrectAmountOfEntries()
        {
            var userBurndown = await _reportService.GetUserBurndown(userId, 30);
            Assert.AreEqual(30, userBurndown.Count);
        }

        [TestMethod]
        public async Task GetUserBurndown_Should_BeValid()
        {
            var userBurndown30 = await _reportService.GetUserBurndown(userId, 30);
            Assert.AreEqual(30, userBurndown30.Count);
            Assert.AreEqual(200, userBurndown30.Last().Total);

            var userBurndown150 = await _reportService.GetUserBurndown(userId, 150);
            Assert.AreEqual(150, userBurndown150.Count);
            Assert.AreEqual(0, userBurndown150.OrderBy(b => b.Date).First().Total);
            Assert.AreEqual(200, userBurndown150.OrderBy(b => b.Date).Last().Total);
        }
    }
}