using MongoDB.Driver;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using RegLab_Test.MongoDB.Entity;

namespace MongoDB_Service.MongoDB
{
    internal class DataContext : DbContext
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
            var connString = GetConnectionUri();
            var client = new MongoClient(connString);
            optionsBuilder.UseMongoDB(client, connString.DatabaseName);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserSettings>().ToCollection("settings");
        }
        private static MongoUrl GetConnectionUri()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile($"appsettings.{env}.json")
                .Build();

            var settings = configuration.GetValue<MongoDbConnectionSetting>("MongoDB");

            var connBuilder = new MongoUrlBuilder
            {
                Username = settings.Username,
                Password = settings.Password,
                DatabaseName = settings.Database,
                Server = new MongoServerAddress(settings.Hostname, settings.Port),

                AuthenticationSource = settings.AuthenticationSource,
            };
            return connBuilder.ToMongoUrl();
        }
    }
}
