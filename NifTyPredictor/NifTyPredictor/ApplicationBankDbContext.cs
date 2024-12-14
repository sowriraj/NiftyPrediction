using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace NifTyPredictor
{
    public class ApplicationBankDbContext : DbContext
    {
        public ApplicationBankDbContext(DbContextOptions<ApplicationBankDbContext> options)
            : base(options)
        {
        }

        public DbSet<Company> BankCompanies { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add any custom model configurations here
        }
    }
}
