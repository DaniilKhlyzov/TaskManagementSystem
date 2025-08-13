using AuthService.Infrastructure.Services;
using FluentAssertions;

namespace AuthService.Tests;

public class PasswordTests
{
    [Fact]
    public void HashPassword_Should_Create_Valid_Hash()
    {
        // Arrange
        var passwordHasher = new PasswordHasher();
        var testPassword = "MySecretPassword123";

        // Act
        var hash = passwordHasher.HashPassword(testPassword);

        // Assert
        hash.Should().NotBeNullOrWhiteSpace();
        hash.Should().NotBe(testPassword); // хеш не должен быть равен паролю
        hash.Length.Should().BeGreaterThan(10); // хеш должен быть достаточно длинным
    }

    [Fact]
    public void VerifyPassword_Should_Return_True_For_Correct_Password()
    {
        // Arrange
        var passwordHasher = new PasswordHasher();
        var testPassword = "MySecretPassword123";
        var hash = passwordHasher.HashPassword(testPassword);

        // Act
        var isValid = passwordHasher.VerifyPassword(hash, testPassword);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_Should_Return_False_For_Wrong_Password()
    {
        // Arrange
        var passwordHasher = new PasswordHasher();
        var correctPassword = "MySecretPassword123";
        var wrongPassword = "WrongPassword456";
        var hash = passwordHasher.HashPassword(correctPassword);

        // Act
        var isValid = passwordHasher.VerifyPassword(hash, wrongPassword);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void HashPassword_Should_Create_Different_Hashes_For_Same_Password()
    {
        // Arrange
        var passwordHasher = new PasswordHasher();
        var testPassword = "MySecretPassword123";

        // Act
        var hash1 = passwordHasher.HashPassword(testPassword);
        var hash2 = passwordHasher.HashPassword(testPassword);

        // Assert
        hash1.Should().NotBe(hash2); // каждый хеш должен быть уникальным
        passwordHasher.VerifyPassword(hash1, testPassword).Should().BeTrue();
        passwordHasher.VerifyPassword(hash2, testPassword).Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void HashPassword_Should_Handle_Empty_Or_Null_Passwords(string password)
    {
        // Arrange
        var passwordHasher = new PasswordHasher();

        // Act & Assert
        FluentActions.Invoking(() => passwordHasher.HashPassword(password))
            .Should().NotThrow();
    }
}


