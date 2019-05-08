using Panda.API.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Panda.API.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategoriesForUserTransactions(Guid userId);

        Task<Category> CreateCategory(Category category);
    }
}