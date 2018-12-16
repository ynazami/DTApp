using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DTApp.API.Data;
using DTApp.API.DTO;
using DTApp.API.Helper;
using DTApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public IDatingRepository _DataRepo { get; }
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository dataRepo, IMapper mapper)
        {
            this._mapper = mapper;
            this._DataRepo = dataRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            int currentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            userParams.UserId = currentUser;
            
            if(string.IsNullOrEmpty(userParams.Gender))
            {
                var userFromRepo = await _DataRepo.GetUser(currentUser);
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";

            }
            var users = await _DataRepo.GetUsers(userParams); 
            var usersToReturn =_mapper.Map<IEnumerable<UserForListDto>>(users);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotlaPages);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}",Name="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _DataRepo.GetUser(id);
            var userToReturn =_mapper.Map<UserForDetailedDto>(user);
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDTO updateUser)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var user = await _DataRepo.GetUser(id);
            _mapper.Map(updateUser, user);
            if(await _DataRepo.SaveAll())
                return NoContent();
            throw new Exception($"Update failed for User {id}");
        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
           if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
           {
                return Unauthorized();
           }

           var like = await _DataRepo.GetLike(id, recipientId);
           if(like != null)
           {
               return BadRequest("You already liked this user");
           }

           var likedUser = await _DataRepo.GetUser(recipientId);

           if(likedUser == null)
           {
               return NotFound();
           }

           like = new Like {
               LikerId = id,
               LikeeId = recipientId
           };

           _DataRepo.Add(like);

           if(await _DataRepo.SaveAll())
           {
               return Ok();
           }
           return BadRequest("Failed to Like User");
        }
    }
}