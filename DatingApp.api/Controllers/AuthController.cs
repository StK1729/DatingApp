using System.Threading.Tasks;
using DatingApp.api.Data;
using DatingApp.api.Models;
using Microsoft.AspNetCore.Mvc;
using DatingApp.api.Dtos;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;

namespace DatingApp.api.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper )
        {
            _repo = repo;
            _config = config;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(/* [FromBody] */UserForRegisterDto userForRegisterDto) 
        {
            // validate request

            /* if(!ModelState.IsValid){
                return BadRequest(ModelState);
            } */

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            if(await _repo.UserExists(userForRegisterDto.Username))
            {
                return BadRequest("Username already exists");
            }
            var UserToCreate = _mapper.Map<User>(userForRegisterDto);

            var createdUser = _repo.Register(UserToCreate, userForRegisterDto.Password);

            var userToReturn = _mapper.Map<UserForDetailedDto>(UserToCreate);
            return CreatedAtRoute("GetUser", new {controller = "Users", Id = createdUser.Id}, userToReturn);
        }
        [HttpPost("login")]

        public async Task<IActionResult> Login(UserForLoginDto userLoginDto)
        {
            var UserFromRepo = await _repo.Login(userLoginDto.Username.ToLower(), userLoginDto.Password);

            if(UserFromRepo == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, UserFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, UserFromRepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token =  tokenHandler.CreateToken(tokenDescriptor);

            var user = _mapper.Map<UserForListDto>(UserFromRepo);

            return Ok(new {
                token = tokenHandler.WriteToken(token),
                user
            });
        }

    }
}