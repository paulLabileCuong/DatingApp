using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection; // Add this line to use CreateScope() method below in Main() method below to create a scope for the seed data to be added to the database when the application starts up (see below)
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build(); // Build the host
            using var scope = host.Services.CreateScope(); // Create a scope
            var services = scope.ServiceProvider; // Get the services from the scope
            try{
                var context = services.GetRequiredService<DataContext>(); // Get the DataContext service from the services

                var userManager = services.GetRequiredService<UserManager<AppUser>>(); // Get the UserManager service from the services

                var roleManager = services.GetRequiredService<RoleManager<AppRole>>(); // Get the RoleManager service from the services
                
                await context.Database.MigrateAsync(); // Migrate the database

                await Seed.SeedUsers(userManager,roleManager); // Seed the users
            }catch(Exception ex){
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured during migration");
            }
        
        await host.RunAsync(); // Run the host
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
