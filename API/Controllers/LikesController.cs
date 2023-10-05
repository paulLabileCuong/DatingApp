using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController // this is the base controller that we created in the BaseApiController.cs
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;

        public LikesController(ILikesRepository likesRepository, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _likesRepository = likesRepository;
        }

        [HttpPost("{username}")] // this is the username of the user that is being liked

        public async Task<ActionResult> AddLike(string username)
        {
            // this is the user that is liking another user
            var sourceUserId = User.GetUserId();
             // this is the user that is being liked
            var likedUser = await _userRepository.GetUserByUsernameAsync(username);
            // this is the user that is liking another user
            var SourceUser = await _likesRepository.GetUserWithLikes(sourceUserId); 

            // if the user that is being liked is null then we are going to return not found
            if(likedUser == null) return NotFound();

            // if the user that is liking another user is the same as the user that is being liked then we are going to return bad request
            if(SourceUser.UserName == username) return BadRequest("You cannot like yourself");

            // this is the like that is being created
            var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);

            // if the user like is not null then we are going to return bad request
            if(userLike != null) return BadRequest("You already like this user");

            // if the user like is null then we are going to create a new user like
            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };
            // we are adding the user like to the database
            SourceUser.LikedUsers.Add(userLike);

            // if the user is saved to the database then we are going to return ok
            if(await _userRepository.SaveAllAsync()) return Ok();
            
            // if the user is not saved to the database then we are going to return bad request
            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {   
            likesParams.UserId = User.GetUserId();
            
            // we are getting the users that are being liked by the user and we are adding the pagination
            var users = await _likesRepository.GetUserLikes(likesParams);

            // we are adding the pagination headers to the response
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, 
                users.TotalCount, users.TotalPages);

            // we are returning the users that are being liked
            return Ok(users);
        } 
    }
}