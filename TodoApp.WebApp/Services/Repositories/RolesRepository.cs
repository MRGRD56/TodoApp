using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApp.DbInterop;
using TodoApp.Infrastructure.Models;

namespace TodoApp.WebApp.Services.Repositories
{
    public class RolesRepository
    {
        private readonly AppDbContext _db;

        public RolesRepository(AppDbContext db)
        {
            _db = db;
        }
        
        public async Task<Role> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _db.Roles.FindAsync(new object[] { id }, cancellationToken);
        }
        
        public async Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _db.Roles.SingleOrDefaultAsync(role => role.Name == name, cancellationToken);
        }
    }
}