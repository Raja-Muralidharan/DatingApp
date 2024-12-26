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

            var unitofWork = resultContext.HttpContext.RequestServices.GetRequiredService<IUnitofWork>();
            var user = await unitofWork.userRepository.GetUserByIdAsync(userID);
            if(user == null) return;
            user.LastActive = DateTime.UtcNow;
            await unitofWork.Complete();
        }
    }
}