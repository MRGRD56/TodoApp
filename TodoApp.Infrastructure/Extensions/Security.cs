using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TodoApp.Infrastructure.Extensions
{
    public static class Security
    {
        private const string Salt = "F9k!na4yx9S!@p/?";
        
        public static byte[] HashPassword(string password)
        {
            var stringHash = BCrypt.Net.BCrypt.HashPassword(password, Salt);
            return Encoding.UTF8.GetBytes(stringHash);
        }

        public static bool VerifyPassword(IEnumerable<byte> correctPasswordHash, string verifiablePassword)
        {
            var verifiablePasswordHash = HashPassword(verifiablePassword);
            return correctPasswordHash.SequenceEqual(verifiablePasswordHash);
        }
    }
}