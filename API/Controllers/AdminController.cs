using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        public AdminController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager) 
        {
            _userManager = userManager;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUserWithRoles()
        {
            var users = await _userManager.Users
                .Include(r => r.UserRoles)
                .ThenInclude(r => r.Role)
                .OrderBy(u => u.UserName)
                .Select(u => new 
                {
                    u.Id,
                    Username = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();
            
            return Ok(users); 
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult>EditRoles(string username, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(",").ToArray(); // chuyển chuỗi roles thành mảng string 

            var user = await _userManager.FindByNameAsync(username); // tìm user theo username

            if(user == null) return NotFound("Could not find user"); // không tìm thấy user

            var userRoles = await _userManager.GetRolesAsync(user); // lấy role của user

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles)); // thêm role mới

            if(!result.Succeeded) return BadRequest("Failed to add to roles"); // thêm role mới thất bại

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles)); // xóa role cũ

            if(!result.Succeeded) return BadRequest("Failed to remove the roles"); // xóa role cũ thất bại

            return Ok(await _userManager.GetRolesAsync(user));  // trả về role mới
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Admins or moderators can see this");
        }
        
    }
}