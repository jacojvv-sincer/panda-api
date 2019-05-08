using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panda.API.Data;
using Panda.API.Data.Models;
using System;
using System.Threading.Tasks;

namespace Panda.API.Services.Tests
{
    [TestClass]
    public class RelationAnalyticsServiceTest
    {
        private ApplicationDbContext _applicationDbContext { get; set; }
        private RelationAnalyticsService _relationAnalyticsService { get; set; }

        private User user;
        private static Guid userId = Guid.NewGuid();
        private Transaction userTransaction;
        private Category groceriesCategory;

        [TestInitialize]
        public void Setup()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("testing_database");
            _applicationDbContext = new ApplicationDbContext(optionsBuilder.Options);

            // Setup groceries category
            groceriesCategory = new Category() { Name = "Groceries" };
            _applicationDbContext.Categories.Add(groceriesCategory);

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
                Category = groceriesCategory,
                Date = DateTime.Now
            };
            _applicationDbContext.Transactions.Add(userTransaction);

            //Save changes
            _applicationDbContext.SaveChanges();

            _relationAnalyticsService = new RelationAnalyticsService(_applicationDbContext);
        }

        [TestCleanup]
        public void TearDown()
        {
            _applicationDbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetRelationAnalytics_Should_ReturnCorrectDataForTheCategoryRelation()
        {
            var analytics = await _relationAnalyticsService.GetRelationAnalytics<Category>(userId, groceriesCategory.Id);
            Assert.AreEqual(-12, analytics.LifetimeAveragePerTransaction);
            Assert.AreEqual(-12, analytics.LifetimeSumOfTransactions);
            Assert.AreEqual(1, analytics.LifetimeTotalTransactions);
            Assert.AreEqual(-12, analytics.MonthSumOfTransactions);
            Assert.AreEqual(-12, analytics.MonthAveragePerTransaction);
            Assert.AreEqual(1, analytics.MonthTotalTransactions);
        }
    }
}