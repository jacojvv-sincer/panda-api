using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyManagerApi.Models
{
    public class Bucket : ModelBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal InitialValue { get; set; }
        public Demarcation Demarcation { get; set; }
        public ICollection<Entry> Entry { get; set; }
    }
}
