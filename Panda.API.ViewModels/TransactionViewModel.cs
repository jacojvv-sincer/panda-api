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
        public bool IsExtraneous { get; set; }
        public decimal Amount { get; set; }
        public CategoryViewModel Category { get; set; }
        public LocationViewModel Location { get; set; }
        public List<TransactionTagViewModel> TransactionTags { get; set; }
        public List<TransactionPersonViewModel> TransactionPeople { get; set; }
    }
}