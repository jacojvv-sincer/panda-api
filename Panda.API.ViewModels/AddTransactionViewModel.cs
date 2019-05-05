using Panda.API.Data.Models;
using System.Collections.Generic;

namespace Panda.API.ViewModels
{
    public class AddTransactionViewModel : Transaction
    {
        public ICollection<Tag> Tags { get; set; }

        public ICollection<Person> People { get; set; }
    }
}