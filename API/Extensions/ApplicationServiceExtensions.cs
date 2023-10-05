using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// this file is for adding services to the application

// what is service? 
// service is a class that we can inject into other classes or components in our application
// for example, we can inject the DataContext into our controller
// we can inject the DataContext into our repository

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings")); // this is the section in appsettings.json
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<ILikesRepository, LikesRepository>(); // this is the interface, this is the implementation
            services.AddScoped<LogUserActivity>(); // this is the class that we created in the Helpers folder to update the last active date
            services.AddScoped<IUserRepository, UserRepository>(); // this is the interface, this is the implementation
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            return services;    
        }
    }
}