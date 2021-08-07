using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TodoApp.Infrastructure.Extensions
{
    public static class Security
    {
        public static byte[] HashPassword(string password)
        {
            var stringHash = BCrypt.Net.BCrypt.HashPassword(password);
            return Encoding.UTF8.GetBytes(stringHash);
        }

        public static bool VerifyPassword(byte[] correctPasswordHash, string verifiablePassword)
        {
            var correctPasswordHashString = Encoding.UTF8.GetString(correctPasswordHash);
            var isValid = BCrypt.Net.BCrypt.Verify(verifiablePassword, correctPasswordHashString);
            return isValid;
        }
    }
}