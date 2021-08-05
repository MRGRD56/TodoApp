using System;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Infrastructure.Models.RequestModels.Auth;

namespace TodoApp.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            throw new NotImplementedException();
        }
    }
}