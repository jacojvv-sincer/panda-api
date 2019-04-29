using Panda.API.Data.Models;
using System.Collections.Generic;

namespace Panda.API.Bindings
{
    public class TransactionBinding : Transaction
    {
        public ICollection<Tag> Tags { get; set; }

        public ICollection<Person> People { get; set; }
    }
}