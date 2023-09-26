using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.Http;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage,
            int ItemsPerPage,int totalItems,int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage,ItemsPerPage,totalItems,totalPages); 
            
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // add the pagination header to the response
            response.Headers.Add("Pagination",JsonSerializer.Serialize(paginationHeader, options));

            // allow the client to access the pagination header's values    
            response.Headers.Add("Access-Control-Expose-Headers","Pagination");
        }
    }
}