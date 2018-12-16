using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DTApp.API.Data;
using DTApp.API.DTO;
using DTApp.API.Helper;
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
    }
}