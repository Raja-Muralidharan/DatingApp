using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extenstions
{
    public static class IdentityServiceExtentions
    {

        public static IServiceCollection AddIdendtityServiceExtention(this IServiceCollection service, 
                                                       IConfiguration config)
        {
            
            service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                            .AddJwtBearer(options =>
                            {
                                var TokenKey = config["TokenKey"] ?? throw new Exception("TokenKey not found");
                                options.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuerSigningKey = true,
                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenKey)),
                                    ValidateIssuer = false,
                                    ValidateAudience = false
                                };
                            }); 

            return service;
        }

    }
}