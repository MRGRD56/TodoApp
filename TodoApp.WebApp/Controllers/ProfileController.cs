using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.DbInterop;
using TodoApp.WebApp.Extensions;

namespace TodoApp.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ProfileController(AppDbContext db)
        {
            _db = db;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var currentUser = await User.GetUserAsync(_db, cancellationToken);
            return Ok(new
            {
                currentUser.Id,
                currentUser.Login,
                Roles = currentUser.Roles.Select(r => r.ToString())
            });
        }
    }
}