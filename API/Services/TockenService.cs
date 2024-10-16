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

namespace API.Services
{
    public class TockenService(IConfiguration config) : ITokenService
    {
        public string CreateToken(AppUser appUser)
        {
            var TokenKey = config["TokenKey"] ?? throw new Exception("Cannot Access TokenKey from AppSettings");
            if(TokenKey.Length < 63) throw new Exception("Your token Key needs to be longer");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenKey));

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, appUser.ID.ToString()),
                new (ClaimTypes.Name, appUser.UserName)
            };

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