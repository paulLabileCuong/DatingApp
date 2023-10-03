using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // this is the filter that we created in the Helpers folder to update the last active date 
    [ServiceFilter(typeof(LogUserActivity))]   

    // this is the base controller for all the controllers in the API project

    [ApiController]

    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        
    }
}