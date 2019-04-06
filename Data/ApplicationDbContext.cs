using System;
using Microsoft.EntityFrameworkCore;
using MoneyManagerApi.Models;

namespace MoneyManagerApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Person> People { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransactionPerson>().HasKey(t => new { t.TransactionId, t.PersonId });
            modelBuilder.Entity<TransactionTag>().HasKey(t => new { t.TransactionId, t.TagId });
        }
    }
}
