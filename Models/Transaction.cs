using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyManagerApi.Models
{
    public class Transaction : ModelBase
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
        public ICollection<Tag> Tags { get; set; }
    }
}
