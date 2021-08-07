using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Infrastructure.Models;
using TodoApp.Infrastructure.Models.Auth;
using TodoApp.Infrastructure.Models.RequestModels.Auth;
using TodoApp.WebApp.Services.Repositories;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace TodoApp.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IOptions<AuthOptions> _authOptions;
        private readonly UsersRepository _usersRepository;

        public AuthController(
            IOptions<AuthOptions> authOptions, 
            UsersRepository usersRepository)
        {
            _authOptions = authOptions;
            _usersRepository = usersRepository;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel, CancellationToken cancellationToken)
        {
            var (login, password) = loginModel;
            var user = await _usersRepository.AuthenticateAsync(login, password, cancellationToken);
            var accessToken = GenerateJwt(user);
            return Ok(new
            {
                user.Id,
                user.Login,
                accessToken
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationModel registrationModel,
            CancellationToken cancellationToken)
        {
            var (login, password) = registrationModel;

            var existingUser = await _usersRepository.GetByLoginAsync(login, cancellationToken);
            if (existingUser != null)
            {
                return Conflict("The user with the specified login already exists.");
            }

            var user = new User(login.Trim(), password, Role.User);
            
            var accessToken = GenerateJwt(user);
            return Ok(new
            {
                user.Id,
                user.Login,
                accessToken
            });
        }

        private string GenerateJwt(User user)
        {
            var authParameters = _authOptions.Value;

            var securityKey = authParameters.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Name, user.Login),
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            };
            
            user.Roles.ForEach(role =>
            {
                claims.Add(new Claim("role", role.ToString()));
            });

            var token = new JwtSecurityToken(
                authParameters.Issuer,
                authParameters.Audience,
                claims,
                expires: DateTime.Now.AddSeconds(authParameters.TokenLifetime),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        } 
    }
}