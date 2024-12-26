using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extenstions
{
    public static class IdentityServiceExtentions
    {

        public static IServiceCollection AddIdendtityServiceExtention(this IServiceCollection service, 
                                                       IConfiguration config)
        {

            service.AddIdentityCore<AppUser>(opt => {
                opt.Password.RequireNonAlphanumeric = false;
             
            })
               .AddRoles<AppRole>()
               .AddRoleManager<RoleManager<AppRole>>()
               .AddEntityFrameworkStores<DataContext>();
            
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

                                options.Events = new JwtBearerEvents
                                {
                                    OnMessageReceived = context => 
                                    {
                                        var accessToken = context.Request.Query["access_token"];

                                        var path = context.HttpContext.Request.Path;
                                        if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                                        {
                                            context.Token = accessToken;
                                        }

                                        return Task.CompletedTask;
                                    }
                                };
                            }); 

            service.AddAuthorizationBuilder()
                .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
                .AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));

            return service;
        }

    }
}