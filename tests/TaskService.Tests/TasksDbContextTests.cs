using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskService.Domain;
using TaskService.Infrastructure;

namespace TaskService.Tests;

public class TaskTests
{
    [Fact]
    public async Task CreateTask_Should_Save_Task_To_Database()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<JobsDbContext>()
            .UseInMemoryDatabase("create-task-test")
            .Options;

        await using var dbContext = new JobsDbContext(options);

        var newTask = new Job 
        { 
            Title = "Тестовая задача", 
            Description = "Описание тестовой задачи",
            Status = JobStatus.Pending
        };

        // Act
        await dbContext.Jobs.AddAsync(newTask);
        await dbContext.SaveChangesAsync();

        // Assert
        var savedTask = await dbContext.Jobs.AsNoTracking().FirstOrDefaultAsync(t => t.Id == newTask.Id);
        savedTask.Should().NotBeNull();
        savedTask!.Title.Should().Be("Тестовая задача");
        savedTask.Description.Should().Be("Описание тестовой задачи");
        savedTask.Status.Should().Be(JobStatus.Pending);
    }

    [Fact]
    public async Task UpdateTask_Should_Modify_Existing_Task()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<JobsDbContext>()
            .UseInMemoryDatabase("update-task-test")
            .Options;

        await using var dbContext = new JobsDbContext(options);

        var task = new Job { Title = "Исходная задача", Status = JobStatus.Pending };
        await dbContext.Jobs.AddAsync(task);
        await dbContext.SaveChangesAsync();

        // Act
        task.Title = "Обновленная задача";
        task.Status = JobStatus.InProgress;
        dbContext.Jobs.Update(task);
        await dbContext.SaveChangesAsync();

        // Assert
        var updatedTask = await dbContext.Jobs.AsNoTracking().FirstAsync(t => t.Id == task.Id);
        updatedTask.Title.Should().Be("Обновленная задача");
        updatedTask.Status.Should().Be(JobStatus.InProgress);
    }

    [Fact]
    public async Task DeleteTask_Should_Remove_Task_From_Database()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<JobsDbContext>()
            .UseInMemoryDatabase("delete-task-test")
            .Options;

        await using var dbContext = new JobsDbContext(options);

        var task = new Job { Title = "Задача для удаления" };
        await dbContext.Jobs.AddAsync(task);
        await dbContext.SaveChangesAsync();

        // Act
        dbContext.Jobs.Remove(task);
        await dbContext.SaveChangesAsync();

        // Assert
        var deletedTask = await dbContext.Jobs.AsNoTracking().FirstOrDefaultAsync(t => t.Id == task.Id);
        deletedTask.Should().BeNull();
    }

    [Fact]
    public async Task SoftDeleteTask_Should_Mark_Task_As_Deleted()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<JobsDbContext>()
            .UseInMemoryDatabase("soft-delete-test")
            .Options;

        await using var dbContext = new JobsDbContext(options);

        var task = new Job { Title = "Задача для мягкого удаления" };
        await dbContext.Jobs.AddAsync(task);
        await dbContext.SaveChangesAsync();

        // Act
        task.IsDeleted = true;
        dbContext.Jobs.Update(task);
        await dbContext.SaveChangesAsync();

        // Assert
        var softDeletedTask = await dbContext.Jobs.AsNoTracking().FirstAsync(t => t.Id == task.Id);
        softDeletedTask.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task AssignTask_Should_Set_Assignee_Id()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<JobsDbContext>()
            .UseInMemoryDatabase("assign-task-test")
            .Options;

        await using var dbContext = new JobsDbContext(options);

        var task = new Job { Title = "Задача для назначения" };
        var assigneeId = Guid.NewGuid();
        await dbContext.Jobs.AddAsync(task);
        await dbContext.SaveChangesAsync();

        // Act
        task.AssigneeId = assigneeId;
        dbContext.Jobs.Update(task);
        await dbContext.SaveChangesAsync();

        // Assert
        var assignedTask = await dbContext.Jobs.AsNoTracking().FirstAsync(t => t.Id == task.Id);
        assignedTask.AssigneeId.Should().Be(assigneeId);
    }

    [Fact]
    public async Task GetTasks_Should_Return_All_Active_Tasks()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<JobsDbContext>()
            .UseInMemoryDatabase("get-tasks-test")
            .Options;

        await using var dbContext = new JobsDbContext(options);

        var task1 = new Job { Title = "Задача 1", IsDeleted = false };
        var task2 = new Job { Title = "Задача 2", IsDeleted = false };
        var deletedTask = new Job { Title = "Удаленная задача", IsDeleted = true };

        await dbContext.Jobs.AddRangeAsync(task1, task2, deletedTask);
        await dbContext.SaveChangesAsync();

        // Act
        var activeTasks = await dbContext.Jobs.AsNoTracking()
            .Where(t => !t.IsDeleted)
            .ToListAsync();

        // Assert
        activeTasks.Should().HaveCount(2);
        activeTasks.Should().Contain(t => t.Title == "Задача 1");
        activeTasks.Should().Contain(t => t.Title == "Задача 2");
        activeTasks.Should().NotContain(t => t.Title == "Удаленная задача");
    }
}


