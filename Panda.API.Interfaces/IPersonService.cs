using Panda.API.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Panda.API.Interfaces
{
    public interface IPersonService
    {
        Task<List<Person>> GetPeopleForUserTransactions(Guid userId);

        Task<Person> CreatePerson(Person category);
    }
}