using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName  { get; set; }
        // This is the password hash
        public byte[] PasswordHash { get; set; }
        // This is the password salt
        public byte[] PasswordSalt { get; set; }

        
    }
}