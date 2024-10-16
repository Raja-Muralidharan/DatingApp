using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extenstions
{
    public static class ApplicationServiceExtenstions
    {

        public static IServiceCollection AddApplicationServics(this IServiceCollection services,
                                                         IConfiguration config)
        {

            services.AddControllers();

            services.AddDbContext<DataContext>(opt =>
            {

                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));

            });

            services.AddCors();
            services.AddScoped<ITokenService, TockenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPhotoService, PhtotoService>();
            services.AddScoped<LogUserActivity>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<CloudnarySettings>(config.GetSection("CloudinarySettings"));
            return services;
        }

    }
}