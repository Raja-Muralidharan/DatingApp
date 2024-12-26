using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Extenstions;
using API.Helpers;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController(IUnitofWork unitofWork): BaseController
    {
        [HttpPost("{targetUserId:int}")]
        public async Task<ActionResult> ToggleLike(int targetUserId)
        {
            var sourceUserId = User.GetUserId();

            if(sourceUserId == targetUserId) return BadRequest("You cannot like yourself");

            var existingLike = await unitofWork.likesRepository.GetUserLike(sourceUserId, targetUserId);

            if(existingLike == null){
                var like = new UserLike
                {
                    SourceUserId = sourceUserId,
                    TargetUserId = targetUserId
                };

                unitofWork.likesRepository.AddLike(like);
            }
            else
            {
               unitofWork.likesRepository.DeleteLike(existingLike);
            }

            if (await unitofWork.Complete()) return Ok();

            return BadRequest("Failed to update like");
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
        {
            return Ok(await unitofWork.likesRepository.GetCurrentUserLikeIds(User.GetUserId()));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes([FromQuery]LikesParams likesparams)
        {
            likesparams.UserId = User.GetUserId();
            var users = await unitofWork.likesRepository.GetUserLikes(likesparams);

            Response.AddPaginationHeader(users);
            return Ok(users);
        }
    }
}