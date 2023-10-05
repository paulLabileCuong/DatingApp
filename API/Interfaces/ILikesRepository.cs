using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId); // user who likes another user
        Task<AppUser> GetUserWithLikes(int userId); // user who likes another user
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams); // user who likes another user
        
    }
}