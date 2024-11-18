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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.Controllers
{
    public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper) : BaseController
    {

        [HttpPost("register")] //account/register

        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {

            if(await UserExist(registerDto.UserName)) return BadRequest("User Already Exist");

            var user = mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.UserName.ToLower();
            
            var result = await userManager.CreateAsync(user, registerDto.password);

            if(!result.Succeeded) return BadRequest(result.Errors);

            return new UserDto
            {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }


        
        [HttpPost("login")]
        
        public async Task<ActionResult<UserDto>> Login(LoginDtos loginDtos)
        {
            var user = await userManager.Users
                  .Include(p => p.Photos)
                  .FirstOrDefaultAsync(x => 
                        x.UserName == loginDtos.UserName.ToLower());

            if( user == null || user.UserName == null) return Unauthorized("Invalid Username");

            var result = await userManager.CheckPasswordAsync(user, loginDtos.password);

            if(!result) return Unauthorized();
            
             return new UserDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Token = await tokenService.CreateToken(user),
                PhotoURL = user.Photos.FirstOrDefault(x => x.IsMain)?.URL,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExist(string UserName)
        {

            return await userManager.Users.AnyAsync( x => x.NormalizedUserName == UserName.ToUpper());
        }

        
    }

   
}