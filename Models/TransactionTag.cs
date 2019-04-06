using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyManagerApi.Models
{
    public class TransactionTag : ModelBase
    {
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
