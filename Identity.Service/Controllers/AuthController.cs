using AutoMapper;
using Identity.Service.Data;
using Identity.Service.JwtService;
using Identity.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using static Identity.Service.Dtos;

namespace Identity.Service.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;

        public AuthController(ApplicationDbContext context, IMapper mapper, IJwtService jwtService)
        {
            _context = context;
            _mapper = mapper;
            _jwtService = jwtService;
        }
        [Authorize]
        [HttpGet]
        public async Task<JsonResult> Get()
        {
            return new JsonResult("HEllo world");
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(UserLoginDto userLoginDto)
        {
            bool isSuccess = await Authenticate(userLoginDto.Telephone, userLoginDto.Password);
            if (isSuccess)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Telephone == userLoginDto.Telephone);
                var token = _jwtService.GenerateToken(user);
                if(token is null)
                {
                    return Unauthorized();
                }
                return Ok(token);
            }
            return StatusCode(StatusCodes.Status401Unauthorized, "Credentials are incorrect or User does not exists");
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(UserRegisterDto userRegisterDto)
        {
            var user = _mapper.Map<User>(userRegisterDto);
            user.Password = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password);
            var registeredUser = await _context.Set<User>().AddAsync(user);
            await _context.SaveChangesAsync();
            if(registeredUser.Entity is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not handle registration!");
            }

            return Ok(registeredUser.Entity);

        }

        private async Task<bool> Authenticate(string telephone, string password)
        {
            var user = await _context.Users.Where(u => u.Telephone == telephone).FirstOrDefaultAsync();
            if(user is null)
            {
                return false;
            }

            if(BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return true;
            }
            return false;
        }

    }
}
