using System;
using System.Collections.Generic;

namespace API.Entities
{
    public class AppUser {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        // Một người dùng có thể có nhiều ảnh
        public ICollection<Photo> Photos { get; set; } // danh sách ảnh của người dùng này 
        public ICollection<UserLike> LikedByUsers { get; set; } // danh sách người dùng đã like mình
        public ICollection<UserLike> LikedUsers { get; set; } // danh sách người dùng mình đã like
    }
}