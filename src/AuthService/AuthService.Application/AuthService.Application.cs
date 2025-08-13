using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Services;

public record RegisterRequestDto(
    [property: Required, EmailAddress] string Email,
    [property: Required, MinLength(2)] string FirstName,
    [property: Required, MinLength(2)] string LastName,
    [property: Required, MinLength(6)] string Password
);

public record LoginRequestDto(
    [property: Required, EmailAddress] string Email,
    [property: Required, MinLength(6)] string Password
);

public record AuthResponseDto(Guid UserId, string Email, string Token);

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<User?> GetCurrentUserAsync(Guid id);
}

public class AuthAppService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService _jwt;

    public AuthAppService(IUserRepository users, IPasswordHasher hasher, IJwtService jwt)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        if (await _users.ExistsAsync(request.Email))
        {
            throw new InvalidOperationException("User already exists");
        }

        var user = new User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = _hasher.HashPassword(request.Password)
        };

        await _users.CreateAsync(user);
        var token = _jwt.GenerateToken(user);
        return new AuthResponseDto(user.Id, user.Email, token);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _users.GetByEmailAsync(request.Email) ?? throw new UnauthorizedAccessException("Invalid credentials");
        if (!_hasher.VerifyPassword(user.PasswordHash, request.Password))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var token = _jwt.GenerateToken(user);
        return new AuthResponseDto(user.Id, user.Email, token);
    }

    public Task<User> GetCurrentUserAsync(Guid id) => _users.GetByIdAsync(id);
}
