using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Repositories;

namespace AuthService.Tests;

public class UserTests
{
    [Fact]
    public async Task CreateUser_Should_Save_User_To_Database()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase("create-user-test")
            .Options;

        await using var dbContext = new AuthDbContext(options);
        var userRepository = new UserRepository(dbContext);

        var newUser = new User
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hashed_password_123",
            Role = "User"
        };

        // Act
        var createdUser = await userRepository.CreateAsync(newUser);

        // Assert
        createdUser.Should().NotBeNull();
        createdUser.Id.Should().NotBeEmpty();
        createdUser.Email.Should().Be("test@example.com");

        // Проверяем, что пользователь действительно сохранен в БД
        var savedUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == createdUser.Id);
        savedUser.Should().NotBeNull();
        savedUser!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetUserByEmail_Should_Return_User_When_Exists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase("get-user-by-email-test")
            .Options;

        await using var dbContext = new AuthDbContext(options);
        var userRepository = new UserRepository(dbContext);

        var testUser = new User
        {
            Email = "existing@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            PasswordHash = "hashed_password",
            Role = "Manager"
        };

        await dbContext.Users.AddAsync(testUser);
        await dbContext.SaveChangesAsync();

        // Act
        var foundUser = await userRepository.GetByEmailAsync("existing@example.com");

        // Assert
        foundUser.Should().NotBeNull();
        foundUser!.Email.Should().Be("existing@example.com");
        foundUser.FirstName.Should().Be("Jane");
        foundUser.Role.Should().Be("Manager");
    }

    [Fact]
    public async Task GetUserByEmail_Should_Return_Null_When_User_Not_Found()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase("get-user-not-found-test")
            .Options;

        await using var dbContext = new AuthDbContext(options);
        var userRepository = new UserRepository(dbContext);

        // Act
        var foundUser = await userRepository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        foundUser.Should().BeNull();
    }

    [Fact]
    public async Task UserExists_Should_Return_True_When_User_Exists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase("user-exists-test")
            .Options;

        await using var dbContext = new AuthDbContext(options);
        var userRepository = new UserRepository(dbContext);

        var testUser = new User
        {
            Email = "exists@example.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "hashed_password"
        };

        await dbContext.Users.AddAsync(testUser);
        await dbContext.SaveChangesAsync();

        // Act
        var exists = await userRepository.ExistsAsync("exists@example.com");

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task UserExists_Should_Return_False_When_User_Not_Exists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase("user-not-exists-test")
            .Options;

        await using var dbContext = new AuthDbContext(options);
        var userRepository = new UserRepository(dbContext);

        // Act
        var exists = await userRepository.ExistsAsync("notexists@example.com");

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task CreateUser_Should_Throw_Exception_When_Email_Already_Exists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase("duplicate-email-test")
            .Options;

        await using var dbContext = new AuthDbContext(options);
        var userRepository = new UserRepository(dbContext);

        var existingUser = new User
        {
            Email = "duplicate@example.com",
            FirstName = "First",
            LastName = "User",
            PasswordHash = "hash1"
        };

        await dbContext.Users.AddAsync(existingUser);
        await dbContext.SaveChangesAsync();

        var duplicateUser = new User
        {
            Email = "duplicate@example.com", // тот же email
            FirstName = "Second",
            LastName = "User",
            PasswordHash = "hash2"
        };

        // Act & Assert
        await FluentActions.Awaiting(() => userRepository.CreateAsync(duplicateUser))
            .Should().ThrowAsync<InvalidOperationException>();
    }
}
