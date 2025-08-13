using System;
using System.Threading.Tasks;
using AuthService.Application.Services;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace AuthService.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task Register_Should_Create_New_User_And_Return_Token()
    {
        // Arrange - подготовка данных
        var mockUserRepo = new Mock<IUserRepository>();
        mockUserRepo.Setup(x => x.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        mockUserRepo.Setup(x => x.CreateAsync(It.IsAny<User>()))
             .ReturnsAsync((User u) => u);

        var mockPasswordHasher = new Mock<IPasswordHasher>();
        mockPasswordHasher.Setup(x => x.HashPassword("testpass")).Returns("hashed_password");

        var mockJwtService = new Mock<IJwtService>();
        mockJwtService.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("jwt_token_123");

        var authService = new AuthAppService(mockUserRepo.Object, mockPasswordHasher.Object, mockJwtService.Object);

        // Act - выполнение действия
        var result = await authService.RegisterAsync(new RegisterRequestDto("test@example.com", "John", "Doe", "testpass"));

        // Assert - проверка результата
        result.Token.Should().Be("jwt_token_123");
        mockUserRepo.Verify(x => x.CreateAsync(It.Is<User>(u => u.Email == "test@example.com" && u.PasswordHash == "hashed_password")), Times.Once);
    }

    [Fact]
    public async Task Login_Should_Return_Token_When_Correct_Password()
    {
        // Arrange
        var testUser = new User { Email = "test@example.com", PasswordHash = "hashed_password" };
        var mockUserRepo = new Mock<IUserRepository>();
        mockUserRepo.Setup(x => x.GetByEmailAsync("test@example.com")).ReturnsAsync(testUser);

        var mockPasswordHasher = new Mock<IPasswordHasher>();
        mockPasswordHasher.Setup(x => x.VerifyPassword("hashed_password", "testpass")).Returns(true);

        var mockJwtService = new Mock<IJwtService>();
        mockJwtService.Setup(x => x.GenerateToken(testUser)).Returns("valid_jwt_token");

        var authService = new AuthAppService(mockUserRepo.Object, mockPasswordHasher.Object, mockJwtService.Object);

        // Act
        var result = await authService.LoginAsync(new LoginRequestDto("test@example.com", "testpass"));

        // Assert
        result.Token.Should().Be("valid_jwt_token");
    }

    [Fact]
    public async Task Login_Should_Throw_Exception_When_User_Not_Found()
    {
        // Arrange
        var mockUserRepo = new Mock<IUserRepository>();
        mockUserRepo.Setup(x => x.GetByEmailAsync("nonexistent@example.com")).ReturnsAsync((User)null);

        var authService = new AuthAppService(mockUserRepo.Object, Mock.Of<IPasswordHasher>(), Mock.Of<IJwtService>());

        // Act & Assert
        await FluentActions.Awaiting(() => authService.LoginAsync(new LoginRequestDto("nonexistent@example.com", "wrongpass")))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Register_Should_Throw_When_User_Already_Exists()
    {
        // Arrange
        var mockUserRepo = new Mock<IUserRepository>();
        mockUserRepo.Setup(x => x.ExistsAsync("existing@example.com")).ReturnsAsync(true);

        var authService = new AuthAppService(mockUserRepo.Object, Mock.Of<IPasswordHasher>(), Mock.Of<IJwtService>());

        // Act & Assert
        await FluentActions.Awaiting(() => authService.RegisterAsync(new RegisterRequestDto("existing@example.com", "John", "Doe", "testpass")))
            .Should().ThrowAsync<InvalidOperationException>();
    }
}
