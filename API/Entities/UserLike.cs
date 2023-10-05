using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class UserLike
    {
        public AppUser SourceUser { get; set; } // user who likes another user
        public int SourceUserId { get; set; } // user who likes another user
        public AppUser LikedUser { get; set; } // user who is liked
        public int LikedUserId { get; set; } // user who is liked

    }
}