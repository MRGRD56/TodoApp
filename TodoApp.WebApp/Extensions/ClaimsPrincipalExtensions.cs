using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.DbInterop;
using TodoApp.Infrastructure.Models;

namespace TodoApp.WebApp.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            var sub = claimsPrincipal.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var userId = int.Parse(sub);
            return userId;
        }

        public static User GetUser(this ClaimsPrincipal claimsPrincipal, AppDbContext appDbContext)
        {
            var userId = claimsPrincipal.GetUserId();
            return appDbContext.Users.Find(userId);
        }
        
        public static async Task<User> GetUserAsync(this ClaimsPrincipal claimsPrincipal, AppDbContext appDbContext,
            CancellationToken cancellationToken = default)
        {
            var userId = claimsPrincipal.GetUserId();
            return await appDbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
        }
    }
}