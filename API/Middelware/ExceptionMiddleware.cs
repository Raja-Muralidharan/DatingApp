using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Erros;

namespace API.Middelware
{
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, 
    IHostEnvironment env)
    {
        public async Task InvoleAsync (HttpContext context)
        {
          
           try
           {
               await next(context);
           }
           catch(Exception ex)
           {
               logger.LogError(ex, ex.Message);
               context.Response.ContentType = "application/json";
               context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

               var responce = env.IsDevelopment()
                     ? new ApiExceptions(context.Response.StatusCode, ex.Message, ex.StackTrace)
                     : new ApiExceptions(context.Response.StatusCode, ex.Message, "Internal Server error");
            
              var options = new JsonSerializerOptions
              {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
              };

              var json = JsonSerializer.Serialize(responce, options);

              await context.Response.WriteAsync(json);
           }
       
        }
    }
}