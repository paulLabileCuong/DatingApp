using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    // [ApiController] : Từ khoá để xác định đây là một API Controller
    [Authorize] // Từ khoá để xác định đây là một API Controller được bảo vệ bởi Authentication
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        // xử lý không đông bộ để tăng tốc độ xử lý chương trình, Từ khoá : Asynchronous
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>>  GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();
            return Ok(users); 
        }
        
        [HttpGet("{username}")]
        public async Task<ActionResult <MemberDto>> GetUsers(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto){
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // lấy username từ token

            var user = await _userRepository.GetUserByUsernameAsync(username);

            _mapper.Map(memberUpdateDto, user); // map từ memberUpdateDto sang user
            _userRepository.Update(user); // update user
            if(await _userRepository.SaveAllAsync()) return NoContent(); // nếu update thành công thì trả về NoContent()
            return BadRequest("Failed to update user"); // nếu update thất bại thì trả về BadRequest()
        }
    }
}