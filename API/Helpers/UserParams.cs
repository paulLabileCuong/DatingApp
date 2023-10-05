using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class UserParams : PaginationParams // inherit from the PaginationParams class
    {

        public string CurrentUsername { get; set; }
        public string Gender { get; set; }

        // the minimum age
        public int MinAge { get; set; } = 18;
        // the maximum age
        public int MaxAge { get; set; } = 150;
        public string OrderBy { get; set; } = "lastActive"; // the default order by is last active
    }
}