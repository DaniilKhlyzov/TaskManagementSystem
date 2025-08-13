using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Integration.Tests;

public class ApiGatewayTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ApiGatewayTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task HealthCheck_Should_Return_Ok()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ApiGateway_Should_Return_NotFound_For_Unknown_Route()
    {
        // Act
        var response = await _client.GetAsync("/api/unknown");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ApiGateway_Should_Handle_Cors_Headers()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/auth/register");
        request.Headers.Add("Origin", "http://localhost:3000");
        request.Headers.Add("Access-Control-Request-Method", "POST");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.Headers.Should().ContainKey("Access-Control-Allow-Origin");
    }

    [Fact]
    public async Task ApiGateway_Should_Forward_Requests_To_Auth_Service()
    {
        // Arrange
        var registerData = new
        {
            email = "test@example.com",
            firstName = "Test",
            lastName = "User",
            password = "TestPassword123"
        };

        var json = JsonSerializer.Serialize(registerData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/register", content);

        // Assert
        // Должен вернуть либо 200 (успех), либо 400 (валидация), но не 404 (сервис недоступен)
        response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ApiGateway_Should_Handle_Json_Requests()
    {
        // Arrange
        var testData = new { message = "test" };
        var json = JsonSerializer.Serialize(testData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/tasks", content);

        // Assert
        // Должен вернуть 401 (Unauthorized) из-за отсутствия JWT, но не 500 (Internal Server Error)
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ApiGateway_Should_Return_Proper_Content_Type()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task ApiGateway_Should_Handle_Large_Requests()
    {
        // Arrange
        var largeData = new { data = new string('x', 10000) }; // 10KB данных
        var json = JsonSerializer.Serialize(largeData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/tasks", content);

        // Assert
        // Не должен падать с ошибкой 413 (Request Entity Too Large)
        response.StatusCode.Should().NotBe(HttpStatusCode.RequestEntityTooLarge);
    }
}

// Вспомогательный класс для тестирования
// Удалено: лишняя точка входа мешает тестовому хосту
