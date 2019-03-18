using System;
using Microsoft.EntityFrameworkCore;
using MoneyManagerApi.Models;

namespace MoneyManagerApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Demarcation> Demarcations { get; set; }
        public DbSet<Bucket> Buckets { get; set; }
        public DbSet<Entry> Entries { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

    }
}



