using AuthService.Domain.Interfaces;

namespace AuthService.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return string.Empty;
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string hashedPassword, string password)
        {
            if (string.IsNullOrEmpty(hashedPassword)) return false;
            return BCrypt.Net.BCrypt.Verify(password ?? string.Empty, hashedPassword);
        }
    }
}


