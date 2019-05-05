using Panda.API.Data.Models;
using Panda.API.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Panda.API.Interfaces
{
    public interface ITransactionService
    {
        Task<int> GetCountOfUserTransactions(Guid userId);

        Task<List<Transaction>> GetUserTransactions(Guid userId, int page = 1, int perPage = 30);

        Task<Transaction> GetUserTransactionById(Guid userId, int id);

        Task<Transaction> SaveTransaction(User user, AddTransactionViewModel model);

        Task<Transaction> UpdateTransaction(User user, AddTransactionViewModel model);

        Task<bool> DeleteTransaction(Guid userId, int transactionId);
    }
}