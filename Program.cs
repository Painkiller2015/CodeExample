
using Microsoft.AspNetCore.Http.Connections;
using RegLab_Test.SignalR;
using AutoMapper;
using MongoDB_Service.MongoDB;
using RegLab_Test.MongoDB.Entity;

namespace RegLab_Test
{
    public class Program
    {
        private static readonly IConfiguration _configuration;
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddDbContext<DataContext>();
            builder.Services.AddAutoMapper(typeof(UserSettings));

            builder.Services.AddOutputCache();
            builder.Services.AddLogging();
            builder.Services.AddSignalR();

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = _configuration.GetValue<string>("CacheSettings: ConnectionString");
                options.InstanceName = _configuration.GetValue<string>("CacheSettings: InstanceName");
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseOutputCache();

            app.UseHttpsRedirection();

            app.MapControllers();

            app.UseAuthorization();

            app.MapHub<SettingsHub>( "/Settings", options =>
            {
                options.Transports = HttpTransportType.LongPolling | HttpTransportType.WebSockets;
            } );

            app.Run();
        }
    }
}
