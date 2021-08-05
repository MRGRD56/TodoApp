using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        public async Task<User> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _db.Users.FindAsync(new object[] {id}, cancellationToken);
            if (user == null)
            {
                throw new BadHttpRequestException("User not found");
            }

            return user;
        }

        public async Task<User> AuthenticateAsync(string login, string password,
            CancellationToken cancellationToken = default)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u =>
                string.Equals(u.Login.Trim(), login.Trim(), StringComparison.InvariantCultureIgnoreCase),
                cancellationToken);
            
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