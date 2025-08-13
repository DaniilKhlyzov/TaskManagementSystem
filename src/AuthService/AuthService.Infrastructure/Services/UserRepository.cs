using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;

namespace AuthService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Data.AuthDbContext _dbContext;

        public UserRepository(Data.AuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetByIdAsync(Guid id) =>
            await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User?> GetByEmailAsync(string email) =>
            await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User> CreateAsync(User user)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Email == user.Email))
            {
                throw new InvalidOperationException("User with this email already exists");
            }
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<bool> ExistsAsync(string email) =>
            await _dbContext.Users.AnyAsync(u => u.Email == email);
    }
}


