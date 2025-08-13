using System;
using System.Threading.Tasks;
using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> ExistsAsync(string email);
    }
    
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string password);
    }
    
    public interface IJwtService
    {
        string GenerateToken(User user);
        Guid? ValidateToken(string token);
    }
}