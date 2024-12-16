using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using RegLab_Test.Logger;
using RegLab_Test.Mongodb.UserSettings.Entity;
using RegLab_Test.MongoDB;
using RegLab_Test.MongoDB.DataBase;
using RegLab_Test.Redis;
using RegLab_Test.SettingsService;
using RegLab_Test.SignalR.Hubs;
using StackExchange.Redis;
using System.Net;

namespace RegLab_Test
{
    public class Program
    {
        private static IConfiguration _configuration;

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            _configuration = builder.Configuration;

            ConfigureServices(builder);

            ConfigureWebHost(builder);

            var app = builder.Build();

            ConfigureMiddleware(app);

            app.Run();
        }

        private static void ConfigureWebHost(WebApplicationBuilder builder)
        {
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(IPAddress.Any, 8080);  
            });
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddSignalR();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SettingsAPI", Version = "v1" });
            });

            ConfigureDatabaseServices(builder);
            ConfigureCacheServices(builder);
            ConfigureApplicationServices(builder);
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                var swaggerConfig = _configuration.GetSection("Swagger");
                var swaggerPath = swaggerConfig.GetValue<string>("path");
                c.SwaggerEndpoint(swaggerPath, "SettingsAPI V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
            //}

            app.UseOutputCache();
            //app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<SettingsHubService>("/settingsHub");
        }

        private static void ConfigureDatabaseServices(WebApplicationBuilder builder)
        {
            // MongoDB            
            builder.Services.AddScoped(serviceProvider =>
            {
                var client = serviceProvider.GetRequiredService<IMongoClient>();
                return client.GetDatabase(_configuration["MongoDBSettings:database"]);
            });
            builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();

            builder.Services.AddDbContext<DataContext>(options =>
            {
                var connString = _configuration.GetConnectionString("MongoDB");
                var database = _configuration["MongoDBSettings:database"];

                options.UseMongoDB(connString, database);
            });
        }

        private static void ConfigureCacheServices(WebApplicationBuilder builder)
        {
            // Redis
            builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(_configuration.GetConnectionString("Redis")));
            builder.Services.Configure<CacheSettings>(_configuration.GetSection("RedisSettings"));
            builder.Services.AddScoped<IRedisService, RedisService>();
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = _configuration.GetValue<string>("CacheSettings:ConnectionString");
                options.InstanceName = _configuration.GetValue<string>("CacheSettings:InstanceName");
            });
        }

        private static void ConfigureApplicationServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ILoggerService, LoggerService>();
            builder.Services.AddScoped<ISettingsService, SettingsService.SettingsService>();
            builder.Services.AddAutoMapper(typeof(UserSettings));
            builder.Services.AddOutputCache();
        }
    }
}
