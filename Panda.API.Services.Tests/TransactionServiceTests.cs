using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panda.API.Data;
using Panda.API.Data.Models;
using Panda.API.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Panda.API.Services.Tests
{
    [TestClass]
    public class TransactionServiceTests
    {
        private ApplicationDbContext _applicationDbContext { get; set; }
        private TransactionService _transactionService { get; set; }

        private User user;
        private static Guid userId = Guid.NewGuid();
        private Transaction userTransaction;
        private static Guid userId2 = Guid.NewGuid();
        private static Guid userId3 = Guid.NewGuid();

        private Category incomeCategory;
        private Category groceriesCategory;

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
                User = user
            };
            _applicationDbContext.Transactions.Add(userTransaction);

            // second user setup
            User user2 = new User()
            {
                Id = userId2
            };
            _applicationDbContext.Users.Add(user2);

            // third user setup
            User user3 = new User()
            {
                Id = userId3
            };
            _applicationDbContext.Users.Add(user3);
            for (int i = 0; i < 100; i++)
            {
                _applicationDbContext.Transactions.Add(new Transaction()
                {
                    User = user3
                });
            }

            // Setup income category
            incomeCategory = new Category() { Name = "Income" };
            _applicationDbContext.Categories.Add(incomeCategory);

            // Setup groceries category
            groceriesCategory = new Category() { Name = "Groceries" };
            _applicationDbContext.Categories.Add(groceriesCategory);

            //Save changes
            _applicationDbContext.SaveChanges();

            _transactionService = new TransactionService(_applicationDbContext);
        }

        [TestCleanup]
        public void TearDown()
        {
            _applicationDbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetCountOfUserTransactions_Should_BeEqualToCountOfUserTransactions()
        {
            var userCountOfTransactions = await _transactionService.GetCountOfUserTransactions(userId);
            Assert.AreEqual(1, userCountOfTransactions);

            var user2CountOfTransactions = await _transactionService.GetCountOfUserTransactions(userId2);
            Assert.AreEqual(0, user2CountOfTransactions);
        }

        [TestMethod]
        public async Task GetUserTransactions_Should_OnlyReturnTransactionsForTheSpecifiedUser()
        {
            var userTransactions = await _transactionService.GetUserTransactions(userId);
            Assert.AreEqual(0, userTransactions.Where(t => t.User.Id != userId).Count());
        }

        [TestMethod]
        public async Task GetUserTransactions_Should_LimitTheAmountOfResultsToTheSpecifiedLimit()
        {
            var user3Transactions = await _transactionService.GetUserTransactions(userId3, 1, 30);
            Assert.AreEqual(30, user3Transactions.Count);
        }

        [TestMethod]
        public async Task GetUserTransactionById_Should_ReturnTheTransactionOfTheUserById()
        {
            var transaction = await _transactionService.GetUserTransactionById(userId, userTransaction.Id);
            Assert.AreEqual(userId, transaction.User.Id);
        }

        [TestMethod]
        public async Task GetUserTransactionById_ShouldNot_ReturnTheTransactionOfTheUserByIdIfTheUserDoesNotOwnTheTransaction()
        {
            var transaction = await _transactionService.GetUserTransactionById(userId2, userTransaction.Id);
            Assert.IsNull(transaction);
        }

        [TestMethod]
        public async Task SaveTransaction_Should_SaveATransactionWithIncomeAsCategoryAsAPositiveAmount()
        {
            var transaction = await _transactionService.SaveTransaction(user, new AddTransactionViewModel()
            {
                Amount = 1,
                Category = incomeCategory
            });

            Assert.IsTrue(transaction.Amount > 0);
        }

        [TestMethod]
        public async Task SaveTransaction_Should_SaveATransactionWithAnyOtherCategoryAsNegativeAmount()
        {
            var transaction = await _transactionService.SaveTransaction(user, new AddTransactionViewModel()
            {
                Amount = 1,
                Category = groceriesCategory
            });

            Assert.IsTrue(transaction.Amount < 0);
        }

        [TestMethod]
        public async Task UpdateTransactions_Should_UpdateTransactionProperties()
        {
            var transaction = await _transactionService.UpdateTransaction(user, new AddTransactionViewModel()
            {
                Id = userTransaction.Id,
                Amount = 1,
                Category = incomeCategory,
                Label = "Updated Label"
            });

            Assert.IsTrue(transaction.Amount > 0);
            Assert.AreEqual(1, transaction.Amount);
            Assert.AreEqual("Updated Label", transaction.Label);
        }

        [TestMethod]
        public async Task DeleteTransaction_Should_DeleteTheTransaction()
        {
            await _transactionService.DeleteTransaction(userId, userTransaction.Id);
            var transaction = await _transactionService.GetUserTransactionById(userId, userTransaction.Id);
            Assert.IsNull(transaction);
        }
    }
}