using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;

        public LikesRepository(DataContext context)
        {
            _context = context;
        }
        
        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            // viet ham nay de lay ra user va danh sach cac user ma user nay da like
            // findasync la tim kiem theo primary key
            return await _context.Likes.FindAsync(sourceUserId, likedUserId); // this is the primary key
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(x => x.UserName).AsQueryable(); // this is the list of users
            var likes = _context.Likes.AsQueryable(); // this is the list of likes

            if(likesParams.Predicate == "liked") // if the user likes another user
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId); // get the list of users that the user likes
                users = likes.Select(like => like.LikedUser); // get the list of users that the user likes
            }

            if(likesParams.Predicate == "likedBy") // if the user is liked by another user
            {
                likes = likes.Where(like => like.LikedUserId == likesParams.UserId); // get the list of users that like the user
                users = likes.Select(like => like.SourceUser); // get the list of users that like the user
            }
            
            // select the list of users that the user likes and map it to the list of like dto objects
            var likedUsers =  users.Select(user => new LikeDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(), // this is an extension method
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url, // this is the main photo
                City = user.City,
                Id = user.Id
            });

            // return the list of liked users
            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, 
                    likesParams.PageSize);
        }

        // lay ra user va danh sach cac user ma user nay da like
        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            // first or default async is to get the user by the user id
            // and include the list of users that the user likes
            return await _context.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        } 
    }
}