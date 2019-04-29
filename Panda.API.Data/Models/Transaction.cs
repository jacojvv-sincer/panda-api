using Panda.API.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Panda.API.Data.Models
{
    public class Transaction : ITimestamps
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Label { get; set; }
        public string Notes { get; set; }
        public DateTime Date { get; set; }
        public Boolean IsExtraneous { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public User User { get; set; }
        public Category Category { get; set; }
        public Location Location { get; set; }
        public ICollection<TransactionTag> TransactionTags { get; set; }
        public ICollection<TransactionPerson> TransactionPeople { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}