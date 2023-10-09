using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUserRole : IdentityUserRole<int> // thêm int để thay thế cho string
    {
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }
}