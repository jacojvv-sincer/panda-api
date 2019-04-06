using System;
using System.Collections.Generic;
using MoneyManagerApi.Models;

namespace MoneyManagerApi.Bindings
{
    public class NewTransactionBinding : Transaction
    {
        public ICollection<Tag> Tags { get; set; }

        public ICollection<Person> People { get; set; }
    }
}
