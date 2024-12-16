using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using RegLab_Test.Mongodb.UserSettings.Entity;

namespace RegLab_Test.MongoDB.DataBase
{
    public class DataContext : DbContext
    {
        public DbSet<UserSettings> UserSettings { get; set; }
        public DataContext()
        {
        }
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserSettings>().ToCollection("settings");
        }
    }
}
