using Panda.API.Contracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Panda.API.Data.Models
{
    public class Location : ITimestamps
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}