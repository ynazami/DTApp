using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DTApp.API.Data;
using DTApp.API.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTApp.API.Controllers
{
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
        public async Task<IActionResult> GetUsers()
        {
            var users = await _DataRepo.GetUsers(); 
            var usersToReturn =_mapper.Map<IEnumerable<UserForListDto>>(users);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _DataRepo.GetUser(id);
            var userToReturn =_mapper.Map<UserForDetailedDto>(user);
            return Ok(userToReturn);
        }
    }
}