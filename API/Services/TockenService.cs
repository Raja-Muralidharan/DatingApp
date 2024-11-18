using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using API.Models;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using SQLitePCL;
using Microsoft.AspNetCore.Identity;

namespace API.Services
{
    public class TockenService(IConfiguration config, UserManager<AppUser> userManager) : ITokenService
    {
        public async Task<string> CreateToken(AppUser appUser)
        {
            var TokenKey = config["TokenKey"] ?? throw new Exception("Cannot Access TokenKey from AppSettings");
            if(TokenKey.Length < 63) throw new Exception("Your token Key needs to be longer");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenKey));

            if(appUser.UserName == null) throw new Exception("No username for user");
            
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
                new (ClaimTypes.Name, appUser.UserName)
            };

            var roles = await userManager.GetRolesAsync(appUser);

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role,role)));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescription);

            return tokenHandler.WriteToken(token);

        }
    }
}