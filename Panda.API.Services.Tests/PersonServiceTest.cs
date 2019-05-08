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
    public class PersonServiceTest
    {
        private ApplicationDbContext _applicationDbContext { get; set; }
        private PersonService _personService { get; set; }

        private User user;
        private static Guid userId = Guid.NewGuid();
        private Transaction userTransaction;
        private Person johnPerson;

        [TestInitialize]
        public void Setup()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("testing_database");
            _applicationDbContext = new ApplicationDbContext(optionsBuilder.Options);

            // Setup groceries category
            johnPerson = new Person() { Name = "John" };
            _applicationDbContext.People.Add(johnPerson);

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

            _applicationDbContext.Add(new TransactionPerson { Transaction = userTransaction, Person = johnPerson });

            //Save changes
            _applicationDbContext.SaveChanges();

            _personService = new PersonService(_applicationDbContext);
        }

        [TestCleanup]
        public void TearDown()
        {
            _applicationDbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetPeopleForUserTransactions_Should_ReturnAllPeopleForUserTransactions()
        {
            var people = await _personService.GetPeopleForUserTransactions(userId);
            Assert.AreEqual(1, people.Count);
            Assert.AreEqual("John", people.First().Name);
        }

        [TestMethod]
        public async Task CreatePerson_Should_PersistThePersonToTheDatabase()
        {
            var person = await _personService.CreatePerson(new Person() { Name = "Jack" });
            Assert.AreEqual(1, _applicationDbContext.People.Where(c => c.Name == "Jack").Count());
        }

        [TestMethod]
        public async Task CreatePerson_Should_ReturnExistingPeopleUponSameCreation()
        {
            var person = await _personService.CreatePerson(johnPerson);
            Assert.AreEqual(person.Id, johnPerson.Id);
        }
    }
}