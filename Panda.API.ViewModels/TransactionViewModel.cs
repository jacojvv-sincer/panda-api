using Panda.API.Data.Models;
using System;
using System.Collections.Generic;

namespace Panda.API.ViewModels
{
    public class TransactionViewModel
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Notes { get; set; }
        public DateTime Date { get; set; }
        public Boolean IsExtraneous { get; set; }
        public decimal Amount { get; set; }
        public User User { get; set; }
        public Category Category { get; set; }
        public Location Location { get; set; }
        public ICollection<TransactionTag> TransactionTags { get; set; }
        public ICollection<TransactionPerson> TransactionPeople { get; set; }
    }
}