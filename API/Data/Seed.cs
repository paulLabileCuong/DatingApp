using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, 
                RoleManager<AppRole> roleManager)
        {
            if(await userManager.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            if(users == null) return;

            var roles = new List<AppRole>
            {
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"}
            }; // tạo ra 3 role 

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            } // tạo role trong database    

            foreach (var user in users){
                
                user.UserName = user.UserName.ToLower(); // chuyển username về chữ thường

                await userManager.CreateAsync(user, "Pa$$w0rd"); // tạo user mới 

                await userManager.AddToRoleAsync(user, "Member"); // thêm user vào role Member
            }

            var admin = new AppUser
            {
                UserName = "admin"
            }; // tạo ra 1 user admin

            await userManager.CreateAsync(admin, "Pa$$w0rd"); // tạo user admin

            await userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"}); // thêm user admin vào role Admin và Moderator
        }
    }
}