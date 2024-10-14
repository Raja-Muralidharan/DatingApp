using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.Controllers
{
    public class AccountController(DataContext context, ITokenService tokenService, IMapper mapper) : BaseController
    {

        [HttpPost("register")] //account/register

        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {

            if(await UserExist(registerDto.UserName)) return BadRequest("User Already Exist");
            using var hmac = new HMACSHA512();


            var user = mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.UserName.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.password));
            user.PasswordSalt = hmac.Key;


            context.Users.Add(user);

            await context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = tokenService.CreateToken(user),
                KnownAs = user.KnownAs
            };
        }


        
        [HttpPost("login")]
        
        public async Task<ActionResult<UserDto>> Login(LoginDtos loginDtos)
        {
            var user = await context.Users
                  .Include(p => p.Photos)
                  .FirstOrDefaultAsync(x => 
                        x.UserName == loginDtos.UserName.ToLower());

            if( user == null) return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDtos.password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

             return new UserDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Token = tokenService.CreateToken(user),
                PhotoURL = user.Photos.FirstOrDefault(x => x.IsMain)?.URL
            };
        }

        private async Task<bool> UserExist(string UserName)
        {

            return await context.Users.AnyAsync( x => x.UserName.ToLower() == UserName.ToLower());
        }

        
    }

   
}