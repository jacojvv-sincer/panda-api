using System;
using System.Collections.Generic;
using Panda.API.Models;

namespace Panda.API.Bindings
{
    public class TransactionBinding : Transaction
    {
        public ICollection<Tag> Tags { get; set; }

        public ICollection<Person> People { get; set; }
    }
}
