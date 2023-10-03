using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {   
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext= await next();

            // if the user is not authenticated, return not thing (do not log the user activity) 
            if(!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            // if the user is authenticated, get the username from the token
            var userId = resultContext.HttpContext.User.GetUserId();

            // lấy user repository từ service container (dependency injection) 
            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();

            // get the user from the database by the username 
            var user = await repo.GetUserByIdAsync(userId);

            // update the last active property of the user
            user.LastActive = DateTime.Now;

            // save the changes to the database
            await repo.SaveAllAsync();
        }
    }
}