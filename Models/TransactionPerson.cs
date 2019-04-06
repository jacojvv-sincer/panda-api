using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyManagerApi.Models
{
    public class TransactionPerson : ModelBase
    {
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}
