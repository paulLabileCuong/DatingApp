using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class PagedList<T> : List<T>
    {
        // this constructor is used to create a new instance of the PagedList class
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize); // round up to the nearest whole number
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items);
        }
        
        // this class is used to return a list of users with pagination
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        // the source parameter is used to pass in the queryable list of users
        // the pageNumber and pageSize parameters are used to pass in the page number and page size
        // the async keyword is used to make the method asynchronous
        // the Task<> class is used to return a list of users asynchronously
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber,
            int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber-1)*pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items,count,pageNumber,pageSize);
        }
    }
}