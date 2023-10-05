using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PaginationParams
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
    }
}