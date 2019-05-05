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
    public class CategoryServiceTests
    {
        private ApplicationDbContext _applicationDbContext { get; set; }
        private CategoryService _categoryService { get; set; }

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
                Category = groceriesCategory
            };
            _applicationDbContext.Transactions.Add(userTransaction);

            //Save changes
            _applicationDbContext.SaveChanges();

            _categoryService = new CategoryService(_applicationDbContext);
        }

        [TestCleanup]
        public void TearDown()
        {
            _applicationDbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetCategoriesForUserTransactions_Should_ReturnAllCategoriesForUserTransactions()
        {
            var categories = await _categoryService.GetCategoriesForUserTransactions(userId);
            Assert.AreEqual(1, categories.Count);
            Assert.AreEqual("Groceries", categories.First().Name);
        }
    }
}