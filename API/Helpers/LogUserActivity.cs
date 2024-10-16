using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using API.Extenstions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if(context.HttpContext.User.Identity?.IsAuthenticated != true) return;

            var userID = resultContext.HttpContext.User.GetUserId();

            var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            var user = await repo.GetUserByIdAsync(userID);
            if(user == null) return;
            user.LastActive = DateTime.UtcNow;
            await repo.SaveAllAsync();
        }
    }
}