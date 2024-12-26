using System.Security.Claims;
using API.DTOs;
using API.Extenstions;
using API.Helpers;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [Authorize]
    public class UsersController(IUnitofWork unitofWork, IMapper mapper, 
          IPhotoService photoService) : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            userParams.CurrentUsername = User.GetUserName();
            var users = await unitofWork.userRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(users);

            return Ok(users);

        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await unitofWork.userRepository.GetMemberAsync(username);

            if(user == null) return NotFound();

            return user;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberDto)
        {
              
              var user = await unitofWork.userRepository.GetUserByUsernameAsync(User.GetUserName());

              if(user == null) return BadRequest("Could not find user");

              mapper.Map(memberDto,user);

              if(await unitofWork.Complete()) return NoContent();

              return BadRequest("Failed to update the user");

        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
           var user = await unitofWork.userRepository.GetUserByUsernameAsync(User.GetUserName());

           if(user == null) return BadRequest("Could not update user");

           var result = await photoService.AddPhotoAsync(file);

           if(result.Error != null) return BadRequest(result.Error.Message);

           var photo = new Photo
           {

            URL = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
           };

           if(user.Photos.Count == 0){
            photo.IsMain = true;
           } 

           user.Photos.Add(photo);

           if(await unitofWork.Complete()) 
                    return CreatedAtAction(nameof(GetUser), 
                    new {username = user.UserName}, mapper.Map<PhotoDto>(photo)) ;

           return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId:int}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await unitofWork.userRepository.GetUserByUsernameAsync(User.GetUserName());

             if(user == null) return BadRequest("Could not find user");

             var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

             if(photo == null || photo.IsMain) return BadRequest("Cannot use this as main photo");

             var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

             if(currentMain != null) currentMain.IsMain = false;
             photo.IsMain = true;

             if(await unitofWork.Complete()) return NoContent();

             return BadRequest("Problem setting main photo");

        }

        [HttpDelete("delete-photo/{photoId:int}")]
        public async Task<ActionResult> DeletePhoto(int photoId){

             var user = await unitofWork.userRepository.GetUserByUsernameAsync(User.GetUserName());

             if(user == null) return BadRequest("Could not found");

             var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

             if(photo == null || photo.IsMain) return BadRequest("This Photo cannot be deleted");

             if(photo.PublicId != null){
                
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error != null) return BadRequest(result.Error.Message);
             }

            user.Photos.Remove(photo);

            if(await unitofWork.Complete()) return Ok();

            return BadRequest("Problem with Deleting");

        }



    }
}