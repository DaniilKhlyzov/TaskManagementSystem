using System;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace AuthService.Tests;

public class JwtTests
{
    private static JwtService CreateJwtService()
    {
        var jwtSettings = new JwtSettings
        {
            SecretKey = new string('k', 32), // минимум 32 символа для безопасности
            Issuer = "TaskManagement.AuthService",
            Audience = "TaskManagement.Client",
            ExpirationHours = 24
        };
        var options = Options.Create(jwtSettings);
        return new JwtService(options);
    }

    [Fact]
    public void GenerateToken_Should_Create_Valid_JWT_Token()
    {
        // Arrange
        var jwtService = CreateJwtService();
        var testUser = new User { Email = "test@example.com", Id = Guid.NewGuid() };

        // Act
        var token = jwtService.GenerateToken(testUser);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Should().Contain("."); // JWT должен содержать точки
    }

    [Fact]
    public void ValidateToken_Should_Return_User_Id_When_Token_Is_Valid()
    {
        // Arrange
        var jwtService = CreateJwtService();
        var testUser = new User { Email = "test@example.com", Id = Guid.NewGuid() };
        var token = jwtService.GenerateToken(testUser);

        // Act
        var userId = jwtService.ValidateToken(token);

        // Assert
        userId.Should().Be(testUser.Id);
    }

    [Fact]
    public void ValidateToken_Should_Throw_Exception_When_Token_Is_Invalid()
    {
        // Arrange
        var jwtService = CreateJwtService();
        var invalidToken = "invalid.jwt.token";

        // Act & Assert
        FluentActions.Invoking(() => jwtService.ValidateToken(invalidToken))
            .Should().Throw<Exception>();
    }

    [Fact]
    public void ValidateToken_Should_Throw_Exception_When_Token_Is_Empty()
    {
        // Arrange
        var jwtService = CreateJwtService();

        // Act & Assert
        FluentActions.Invoking(() => jwtService.ValidateToken(""))
            .Should().Throw<Exception>();
    }
}


