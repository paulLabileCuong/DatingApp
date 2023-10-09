using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,
            IConfiguration config)
        {
            // Đăng ký Identity Service
            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false; // không bắt buộc phải có ký tự đặc biệt
            })
                .AddRoles<AppRole>() // đăng ký thêm Identity Role
                .AddRoleManager<RoleManager<AppRole>>() // đăng ký thêm RoleManager
                .AddSignInManager<SignInManager<AppUser>>() // đăng ký thêm SignInManager
                .AddRoleValidator<RoleValidator<AppRole>>() // đăng ký thêm RoleValidator
                .AddEntityFrameworkStores<DataContext>(); // đăng ký thêm EntityFrameworkStores

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(option => 
                {
                    option.TokenValidationParameters = new TokenValidationParameters{
                        
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
 
            // Đăng ký Policy
            services.AddAuthorization(opt => 
            {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
            });

            return services;
        }
    }
}