using Microsoft.EntityFrameworkCore;
using RealState.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RealState.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        // Define a DbSet for the User model
        public DbSet<Contact> Contact { get; set; }

        public DbSet<Users> Users { get; set; }

        // Optionally, you can override OnModelCreating for further configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
