using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
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

            return services;
        }

    }
}