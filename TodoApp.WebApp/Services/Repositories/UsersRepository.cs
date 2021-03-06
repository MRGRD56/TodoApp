using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApp.DbInterop;
using TodoApp.Infrastructure.Models;

namespace TodoApp.WebApp.Services.Repositories
{
    public class UsersRepository
    {
        private readonly AppDbContext _db;

        public UsersRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _db.Users.FindAsync(new object[] {id}, cancellationToken);
        }

        public async Task<User> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
        {
            var users = await EntityFrameworkQueryableExtensions.ToListAsync(_db.Users, cancellationToken);
            var user = users.SingleOrDefault(u =>
                    string.Equals(u.Login.Trim(), login.Trim(), StringComparison.InvariantCultureIgnoreCase));

            return user;
        }

        public async Task<User> AuthenticateAsync(string login, string password,
            CancellationToken cancellationToken = default)
        {
            var user = await GetByLoginAsync(login, cancellationToken);

            if (user == null)
            {
                throw new HttpRequestException("The user with the specified login does not exist",
                    null, HttpStatusCode.Unauthorized);
            }

            if (!user.VerifyPassword(password))
            {
                throw new HttpRequestException("Invalid password",
                    null, HttpStatusCode.Unauthorized);
            }

            return user;
        }
    }
}