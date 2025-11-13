using AnimalCatalog.API.Data.Mappings;
using AnimalCatalog.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace AnimalCatalog.API.Data.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Animal> Animals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AnimalMap());
        }
    }
}