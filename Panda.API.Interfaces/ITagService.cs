using Panda.API.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Panda.API.Interfaces
{
    public interface ITagService
    {
        Task<List<Tag>> GetTagsForUserTransactions(Guid userId);

        Task<Tag> CreateTag(Tag tag);
    }
}