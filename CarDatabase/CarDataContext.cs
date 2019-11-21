using CarDatabase.Model;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Reflection;

namespace CarDatabase
{
    class CarDataContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Note that this time we play with SQLite. You can find a browser
            // at https://sqlitebrowser.org/dl/.
            var dbFileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mydb.db");
            optionsBuilder.UseSqlite($"Data Source={dbFileName};");
        }

        public DbSet<CarMake> CarMakes { get; set; }

        public DbSet<CarModel> CarModels { get; set; }

        public DbSet<Ownership> Ownerships { get; set; }

        public DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Define foreign keys to book and author
            modelBuilder.Entity<Person>()
                .HasMany(p => p.Cars)
                .WithOne(p => p.Person);
            modelBuilder.Entity<CarModel>()
                .HasMany(c => c.Owners)
                .WithOne(c => c.CarModel);

            modelBuilder.Entity<Ownership>()
                .HasIndex(o => o.VehicleIdentificationNumber)
                .IsUnique();
        }
    }
}