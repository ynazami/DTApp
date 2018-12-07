using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DTApp.API.Data;
using DTApp.API.DTO;
using DTApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DTApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repository, IConfiguration config)
        {
            this._config = config;
            this._repository = repository;

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO user)
        {
            var userData = await _repository.Login(user.Username.ToLower(), user.Password);

            if (userData == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userData.Id.ToString()),
                new Claim(ClaimTypes.Name, userData.Username.ToString())

            };
            var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescrip = new SecurityTokenDescriptor
            {
                Subject=new ClaimsIdentity( claims),
                Expires=DateTime.Now.AddDays(1),
                SigningCredentials = cred
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescrip);
            return Ok(new {token = tokenHandler.WriteToken(token)});

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto user)
        {
            //Validate

            user.Username = user.Username.ToLower();

            if (await _repository.UserExists(user.Username))
            {
                return BadRequest("Username already exists");
            }

            var userToCreate = new User
            {
                Username = user.Username
            };

            userToCreate = await _repository.Register(userToCreate, user.Password);

            return StatusCode(201);
        }
    }
}