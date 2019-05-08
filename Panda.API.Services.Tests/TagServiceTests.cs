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
    public class TagServiceTests
    {
        private ApplicationDbContext _applicationDbContext { get; set; }
        private TagService _tagService { get; set; }

        private User user;
        private static Guid userId = Guid.NewGuid();
        private Transaction userTransaction;
        private Tag greenTag;

        [TestInitialize]
        public void Setup()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("testing_database");
            _applicationDbContext = new ApplicationDbContext(optionsBuilder.Options);

            // Setup groceries category
            greenTag = new Tag() { Name = "Green" };
            _applicationDbContext.Tags.Add(greenTag);

            // user setup
            user = new User()
            {
                Id = userId
            };
            _applicationDbContext.Users.Add(user);
            userTransaction = new Transaction()
            {
                User = user,
                Amount = -12
            };
            _applicationDbContext.Transactions.Add(userTransaction);

            _applicationDbContext.Add(new TransactionTag { Transaction = userTransaction, Tag = greenTag });

            //Save changes
            _applicationDbContext.SaveChanges();

            _tagService = new TagService(_applicationDbContext);
        }

        [TestCleanup]
        public void TearDown()
        {
            _applicationDbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetTagsForUserTransactions_Should_ReturnAllTagsForUserTransactions()
        {
            var tags = await _tagService.GetTagsForUserTransactions(userId);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual("Green", tags.First().Name);
        }

        [TestMethod]
        public async Task CreateTag_Should_PersistTheTagToTheDatabase()
        {
            var tag = await _tagService.CreateTag(new Tag() { Name = "RandomTagName" });
            Assert.AreEqual(1, _applicationDbContext.Tags.Where(c => c.Name == "RandomTagName").Count());
        }

        [TestMethod]
        public async Task CreateTag_Should_ReturnExistingTagsUponSameCreation()
        {
            var tag = await _tagService.CreateTag(greenTag);
            Assert.AreEqual(tag.Id, greenTag.Id);
        }
    }
}