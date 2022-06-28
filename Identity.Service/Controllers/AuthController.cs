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

        [HttpPost]
        [Route("user/login")]
        public async Task<ActionResult> Login(UserLoginDto userLoginDto)
        {
            bool isSuccess = await AuthenticateUser(userLoginDto.Telephone, userLoginDto.Password);
            if (isSuccess)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Telephone == userLoginDto.Telephone);
                var token = _jwtService.GenerateTokenForUser(user);
                if(token is null)
                {
                    return Unauthorized();
                }
                return Ok(token);
            }
            return StatusCode(StatusCodes.Status401Unauthorized, "Credentials are incorrect or User does not exists");
        }

        [HttpPost]
        [Route("user/register")]
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

        [HttpPost]
        [Route("company/login")]
        public async Task<ActionResult> LoginCompany(CompanyLoginDto companyLoginDto)
        {
            bool isSuccess = await AuthenticateCompany(companyLoginDto.companyLogin, companyLoginDto.companyPassword);
            if (isSuccess)
            {
                var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyLogin == companyLoginDto.companyLogin);
                var token = _jwtService.GenerateTokenForCompany(company);
                if (token is null)
                {
                    return Unauthorized();
                }
                return Ok(token);
            }
            return StatusCode(StatusCodes.Status401Unauthorized, "Credentials are incorrect or Company does not exists");
        }

        [HttpPost]
        [Route("company/register")]
        public async Task<ActionResult> RegisterCompany(CompanyRegisterDto companyRegisterDto)
        {
            var company = _mapper.Map<Company>(companyRegisterDto);
            company.CompanyPassword = BCrypt.Net.BCrypt.HashPassword(companyRegisterDto.CompanyPassword);
            var registeredCompany = await _context.Set<Company>().AddAsync(company);
            await _context.SaveChangesAsync();
            if (registeredCompany.Entity is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not handle registration!");
            }

            return Ok(registeredCompany.Entity);

        }

        private async Task<bool> AuthenticateUser(string telephone, string password)
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

        private async Task<bool> AuthenticateCompany(string companyLogin, string companyPassword)
        {
            var company = await _context.Companies.Where(c => c.CompanyLogin == companyLogin).FirstOrDefaultAsync();
            if(company is null)
            {
                return false;
            }

            if(BCrypt.Net.BCrypt.Verify(companyPassword, company.CompanyPassword))
            {
                return true;
            }
            return false;
        }

    }
}
