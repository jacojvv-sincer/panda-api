using Panda.API.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Panda.API.Models
{
    public class TransactionPerson : ITimestamps
    {
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
