using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Errors
{
    public class ApiException
    {
        public ApiException(int stutusCode, string message = null, string details = null)
        {
            StutusCode = stutusCode;
            Message = message;
            Details = details;
        }
        
        public int StutusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}