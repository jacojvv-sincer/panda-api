using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyManagerApi.Models
{
    public class Entry : ModelBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public Boolean IsExtraneous { get; set; }
        public decimal Amount { get; set; }
        public Bucket Bucket { get; set; }
    }
}
