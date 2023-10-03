using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 50; // the maximum page size is 50
        public int PageNumber { get; set; } = 1; // the default page number is 1
        private int _pageSize = 10; // the default page size is 10
        public int PageSize
        {
            get => _pageSize;
            // if the value is greater than the max page size, set the page size to the max page size, otherwise set it to the value
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value; 
        }

        public string CurrentUsername { get; set; }
        public string Gender { get; set; }

        // the minimum age
        public int MinAge { get; set; } = 18;
        // the maximum age
        public int MaxAge { get; set; } = 150;
        public string OrderBy { get; set; } = "lastActive"; // the default order by is last active
    }
}