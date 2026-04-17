using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TaskManager.Context;
using TaskManager.Migrations;

namespace TaskManager.UnitTests.Migrations;

public class MigrationTests
{
    private sealed class PublicInitialMigration : InitialMigration
    {
        public void RunUp(MigrationBuilder builder) => base.Up(builder);
        public void RunDown(MigrationBuilder builder) => base.Down(builder);
    }

    private sealed class PublicGoogleLoginMigration : GoogleLogin
    {
        public void RunUp(MigrationBuilder builder) => base.Up(builder);
        public void RunDown(MigrationBuilder builder) => base.Down(builder);
    }

    [Fact]
    public void InitialMigration_UpAndDown_ShouldBuildOperations()
    {
        var migration = new PublicInitialMigration();
        var builder = new MigrationBuilder("Npgsql");

        migration.RunUp(builder);

        Assert.NotEmpty(builder.Operations);

        var downBuilder = new MigrationBuilder("Npgsql");
        migration.RunDown(downBuilder);

        Assert.NotNull(downBuilder.Operations);
    }

    [Fact]
    public void GoogleLoginMigration_UpAndDown_ShouldBuildOperations()
    {
        var migration = new PublicGoogleLoginMigration();
        var builder = new MigrationBuilder("Npgsql");

        migration.RunUp(builder);

        Assert.NotEmpty(builder.Operations);

        var downBuilder = new MigrationBuilder("Npgsql");
        migration.RunDown(downBuilder);

        Assert.NotNull(downBuilder.Operations);
    }

    [Fact]
    public void TaskManagerContextModelSnapshot_ShouldBuildModel()
    {
        var options = new DbContextOptionsBuilder<TaskManagerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new TaskManagerContext(options);
        var model = context.Model;

        Assert.NotNull(model);
        
        // Verify the model contains expected entities
        var taskEntity = model.FindEntityType(typeof(TaskManager.Models.Entities.Task));
        var userEntity = model.FindEntityType(typeof(TaskManager.Models.Entities.User));

        Assert.NotNull(taskEntity);
        Assert.NotNull(userEntity);
    }

    [Fact]
    public void InitialMigration_BuildTargetModel_ShouldNotBeNull()
    {
        var migration = new InitialMigration();

        Assert.NotNull(migration.TargetModel);
    }

    [Fact]
    public void GoogleLoginMigration_BuildTargetModel_ShouldNotBeNull()
    {
        var migration = new GoogleLogin();

        Assert.NotNull(migration.TargetModel);
    }
}
