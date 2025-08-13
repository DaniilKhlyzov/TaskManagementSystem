using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.API.Controllers;
using NotificationService.Domain;
using NotificationService.Infrastructure;
using Moq;
using NotificationService.API.Services;
using NotificationService.Application.Notifications.Commands;

namespace NotificationService.Tests;

public class NotificationTests
{
    [Fact]
    public async Task GetUserNotifications_Should_Return_User_Notifications()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<NotificationDbContext>()
            .UseInMemoryDatabase("get-notifications-test")
            .Options;

        await using var dbContext = new NotificationDbContext(options);
        var mockNotificationSender = new Mock<INotificationSender>();
        var controller = new NotificationsController(dbContext, mockNotificationSender.Object);

        var userId = Guid.NewGuid();
        var notification1 = new Notification { UserId = userId, Title = "Уведомление 1", Message = "Текст 1" };
        var notification2 = new Notification { UserId = userId, Title = "Уведомление 2", Message = "Текст 2" };
        var otherUserNotification = new Notification { UserId = Guid.NewGuid(), Title = "Чужое уведомление", Message = "Текст" };

        await dbContext.Notifications.AddRangeAsync(notification1, notification2, otherUserNotification);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await controller.List(userId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var notifications = okResult.Value.Should().BeAssignableTo<IEnumerable<Notification>>().Subject;
        notifications.Should().HaveCount(2);
        notifications.Should().Contain(n => n.Title == "Уведомление 1");
        notifications.Should().Contain(n => n.Title == "Уведомление 2");
    }

    [Fact]
    public async Task CreateNotification_Should_Save_And_Send_Notification()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<NotificationDbContext>()
            .UseInMemoryDatabase("create-notification-test")
            .Options;

        await using var dbContext = new NotificationDbContext(options);
        var mockNotificationSender = new Mock<INotificationSender>();
        var controller = new NotificationsController(dbContext, mockNotificationSender.Object);

        var newNotification = new Notification
        {
            UserId = Guid.NewGuid(),
            Title = "Новое уведомление",
            Message = "Текст нового уведомления"
        };

        // Act
        var result = await controller.Create(newNotification);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var createdNotification = createdResult.Value.Should().BeOfType<Notification>().Subject;
        createdNotification.Title.Should().Be("Новое уведомление");
        createdNotification.Id.Should().NotBeEmpty();

        // Проверяем, что уведомление сохранено в БД
        var savedNotification = await dbContext.Notifications.FirstOrDefaultAsync(n => n.Id == createdNotification.Id);
        savedNotification.Should().NotBeNull();

        // Проверяем, что метод отправки был вызван
        mockNotificationSender.Verify(x => x.SendToUserAsync(newNotification.UserId, It.IsAny<object>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateNotification_Should_Return_BadRequest_When_Title_Is_Empty()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<NotificationDbContext>()
            .UseInMemoryDatabase("create-invalid-notification-test")
            .Options;

        await using var dbContext = new NotificationDbContext(options);
        var mockNotificationSender = new Mock<INotificationSender>();
        var controller = new NotificationsController(dbContext, mockNotificationSender.Object);

        var invalidNotification = new Notification
        {
            UserId = Guid.NewGuid(),
            Title = "", // пустой заголовок
            Message = "Текст уведомления"
        };

        // Act
        var result = await controller.Create(invalidNotification);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetUserNotifications_Should_Return_Empty_List_For_New_User()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<NotificationDbContext>()
            .UseInMemoryDatabase("empty-notifications-test")
            .Options;

        await using var dbContext = new NotificationDbContext(options);
        var mockNotificationSender = new Mock<INotificationSender>();
        var controller = new NotificationsController(dbContext, mockNotificationSender.Object);

        var newUserId = Guid.NewGuid();

        // Act
        var result = await controller.List(newUserId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var notifications = okResult.Value.Should().BeAssignableTo<IEnumerable<Notification>>().Subject;
        notifications.Should().BeEmpty();
    }
}
