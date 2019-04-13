using Panda.API.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Panda.API.Models
{
    public class TransactionTag : ITimestamps
    {
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
