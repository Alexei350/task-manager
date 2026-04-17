using TaskManager.Models.Base;
using TaskManager.Models.Entities;
using TaskManager.Models.Enums;
using TaskManager.Models.Request.Authentication;
using TaskManager.Models.Request.Task;
using TaskManager.Models.Request.User;
using TaskManager.Models.Return;
using Task = TaskManager.Models.Entities.Task;

namespace TaskManager.UnitTests.ModelTests;

public class ModelsTests
{
    [Fact]
    public void User_ShouldSetAndGetProperties()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test Name",
            Email = "test@email.com",
            Password = "hash",
            Salt = "salt",
            Role = UserRoleEnum.Admin,
            RefreshToken = "token",
            LastAccess = DateTime.Now,
            Deleted = false,
            Culture = "en-US",
            GoogleUserId = "google123",
            Tasks = new List<Task>()
        };

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("Test Name", user.Name);
        Assert.Equal("test@email.com", user.Email);
        Assert.Equal("hash", user.Password);
        Assert.Equal("salt", user.Salt);
        Assert.Equal(UserRoleEnum.Admin, user.Role);
        Assert.Equal("token", user.RefreshToken);
        Assert.NotNull(user.LastAccess);
        Assert.False(user.Deleted);
        Assert.Equal("en-US", user.Culture);
        Assert.Equal("google123", user.GoogleUserId);
        Assert.NotNull(user.Tasks);
    }

    [Fact]
    public void Task_ShouldSetAndGetProperties()
    {
        var task = new Task
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Description = "Desc",
            Observation = "Obs",
            Status = TaskStatusEnum.Finished,
            CreationDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(1),
            TimeSpent = TimeSpan.FromHours(1),
            User = new User()
        };

        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.NotEqual(Guid.Empty, task.UserId);
        Assert.Equal("Desc", task.Description);
        Assert.Equal("Obs", task.Observation);
        Assert.Equal(TaskStatusEnum.Finished, task.Status);
        Assert.NotEqual(DateTime.MinValue, task.CreationDate);
        Assert.NotNull(task.DueDate);
        Assert.NotNull(task.TimeSpent);
        Assert.NotNull(task.User);
    }

    [Fact]
    public void LoginModel_ShouldSetAndGetProperties()
    {
        var model = new LoginModel
        {
            Email = "email",
            Password = "password"
        };

        Assert.Equal("email", model.Email);
        Assert.Equal("password", model.Password);
    }

    [Fact]
    public void RefreshTokenModel_ShouldSetAndGetProperties()
    {
        var model = new RefreshTokenModel
        {
            RefreshToken = "token",
            Token = "jwt"
        };

        Assert.Equal("token", model.RefreshToken);
        Assert.Equal("jwt", model.Token);
    }

    [Fact]
    public void GoogleLoginModel_ShouldSetAndGetProperties()
    {
        var model = new GoogleLoginModel
        {
            Token = "token"
        };

        Assert.Equal("token", model.Token);
    }

    [Fact]
    public void ReturnData_ReturnInfo_ShouldSetCorrectProperties()
    {
        var result = ReturnData.ReturnInfo("Info message");

        Assert.True(result.Success);
        Assert.Single(result.Messages);
        Assert.Equal(ReturnMessageTypeEnum.Info, result.Messages[0].Type);
        Assert.Equal("Info message", result.Messages[0].Message);
    }

    [Fact]
    public void ReturnData_ReturnWarning_ShouldSetCorrectProperties()
    {
        var result = ReturnData.ReturnWarning("Warning message");

        Assert.False(result.Success);
        Assert.Single(result.Messages);
        Assert.Equal(ReturnMessageTypeEnum.Warning, result.Messages[0].Type);
        Assert.Equal("Warning message", result.Messages[0].Message);
    }

    [Fact]
    public void ReturnDataGeneric_ReturnInfo_ShouldSetCorrectProperties()
    {
        var result = ReturnData<string>.ReturnInfo("Info message");

        Assert.True(result.Success);
        Assert.Single(result.Messages);
        Assert.Equal(ReturnMessageTypeEnum.Info, result.Messages[0].Type);
        Assert.Equal("Info message", result.Messages[0].Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public void ReturnDataGeneric_ReturnSuccess_ShouldSetCorrectProperties()
    {
        var result = ReturnData<string>.ReturnSuccess("Success message");

        Assert.True(result.Success);
        Assert.Single(result.Messages);
        Assert.Equal(ReturnMessageTypeEnum.Success, result.Messages[0].Type);
        Assert.Equal("Success message", result.Messages[0].Message);
    }

    [Fact]
    public void ReturnDataGeneric_ReturnWarning_ShouldSetCorrectProperties()
    {
        var result = ReturnData<string>.ReturnWarning("Warning message");

        Assert.False(result.Success);
        Assert.Single(result.Messages);
        Assert.Equal(ReturnMessageTypeEnum.Warning, result.Messages[0].Type);
        Assert.Equal("Warning message", result.Messages[0].Message);
    }

    [Fact]
    public void ReturnDataGeneric_ReturnError_ShouldSetCorrectProperties()
    {
        var result = ReturnData<int>.ReturnError("Error message");

        Assert.False(result.Success);
        Assert.Single(result.Messages);
        Assert.Equal(ReturnMessageTypeEnum.Error, result.Messages[0].Type);
        Assert.Equal("Error message", result.Messages[0].Message);
    }

    [Fact]
    public void ReturnDataGeneric_ShouldStoreData()
    {
        var result = new ReturnData<string>
        {
            Success = true,
            Data = "test data",
            Messages = new List<ReturnMessage>()
        };

        Assert.True(result.Success);
        Assert.Equal("test data", result.Data);
        Assert.Empty(result.Messages);
    }

    [Fact]
    public void CreateTaskModel_ShouldSetAndGetProperties()
    {
        var model = new CreateTaskModel
        {
            Description = "Desc",
            Observation = "Obs",
            Status = TaskStatusEnum.Pending,
            DueDate = DateTime.Now,
            TimeSpent = TimeSpan.Zero
        };

        Assert.Equal("Desc", model.Description);
        Assert.Equal("Obs", model.Observation);
        Assert.Equal(TaskStatusEnum.Pending, model.Status);
        Assert.NotNull(model.DueDate);
        Assert.Equal(TimeSpan.Zero, model.TimeSpent);
    }

    [Fact]
    public void UpdateTaskModel_ShouldSetAndGetProperties()
    {
        var model = new UpdateTaskModel
        {
            Id = Guid.NewGuid(),
            Description = "Desc",
            Observation = "Obs",
            Status = TaskStatusEnum.Pending,
            DueDate = DateTime.Now,
            TimeSpent = TimeSpan.Zero
        };

        Assert.NotEqual(Guid.Empty, model.Id);
        Assert.Equal("Desc", model.Description);
        Assert.Equal("Obs", model.Observation);
        Assert.Equal(TaskStatusEnum.Pending, model.Status);
        Assert.NotNull(model.DueDate);
        Assert.Equal(TimeSpan.Zero, model.TimeSpent);
    }

    [Fact]
    public void CreateUserModel_ShouldSetAndGetProperties()
    {
        var model = new CreateUserModel
        {
            Name = "Name",
            Email = "Email",
            Password = "Pass",
            GoogleUserId = "GoogleId"
        };

        Assert.Equal("Name", model.Name);
        Assert.Equal("Email", model.Email);
        Assert.Equal("Pass", model.Password);
        Assert.Equal("GoogleId", model.GoogleUserId);
    }

    [Fact]
    public void UpdateUserModel_ShouldSetAndGetProperties()
    {
        var model = new UpdateUserModel
        {
            Name = "Name"
        };

        Assert.Equal("Name", model.Name);
    }

    [Fact]
    public void TaskReturn_ShouldSetAndGetProperties()
    {
        var model = new TaskReturn
        {
            Id = Guid.NewGuid(),
            Description = "Desc",
            Observation = "Obs",
            Status = TaskStatusEnum.Pending,
            CreationDate = DateTime.Now,
            DueDate = DateTime.Now,
            TimeSpent = TimeSpan.Zero
        };

        Assert.NotEqual(Guid.Empty, model.Id);
        Assert.Equal("Desc", model.Description);
        Assert.Equal("Obs", model.Observation);
        Assert.Equal(TaskStatusEnum.Pending, model.Status);
        Assert.NotEqual(DateTime.MinValue, model.CreationDate);
        Assert.NotNull(model.DueDate);
        Assert.Equal(TimeSpan.Zero, model.TimeSpent);
    }

    [Fact]
    public void TokenReturn_ShouldSetAndGetProperties()
    {
        var model = new TokenReturn
        {
            Token = "Token",
            RefreshToken = "Refresh",
            User = new UserReturn()
        };

        Assert.Equal("Token", model.Token);
        Assert.Equal("Refresh", model.RefreshToken);
        Assert.NotNull(model.User);
    }

    [Fact]
    public void UserReturn_ShouldSetAndGetProperties()
    {
        var model = new UserReturn
        {
            Id = Guid.NewGuid(),
            Name = "Name",
            Email = "Email"
        };

        Assert.NotEqual(Guid.Empty, model.Id);
        Assert.Equal("Name", model.Name);
        Assert.Equal("Email", model.Email);
    }
}
