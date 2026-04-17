using Microsoft.EntityFrameworkCore;
using TaskManager.Context;
using TaskManager.Models.Base.Query;
using TaskManager.Models.Enums;
using TaskManager.Repository;

namespace TaskManager.UnitTests.Repository
{
    public class TaskRepositoryTests : IDisposable
    {
        private readonly TaskManagerContext _context;
        private readonly TaskRepository _repository;

        public TaskRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TaskManagerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TaskManagerContext(options);
            _repository = new TaskRepository(_context);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddTaskToDatabase()
        {
            // Arrange
            var todoItem = new Models.Entities.Task
            {
                Description = "Test Description",
                Status = TaskStatusEnum.Pending,
                UserId = Guid.NewGuid()
            };

            // Act
            await _repository.CreateAsync(todoItem);
            await _context.SaveChangesAsync();

            // Assert
            var savedItem = await _context.Tasks.FirstOrDefaultAsync(t => t.Description == "Test Description");
            Assert.NotNull(savedItem);
            Assert.Equal("Test Description", savedItem.Description);
        }

        [Fact]
        public void Update_ShouldModifyTask()
        {
            // Arrange
            var todoItem = new Models.Entities.Task
            {
                Description = "Original Description",
                Status = TaskStatusEnum.Pending,
                UserId = Guid.NewGuid()
            };

            _context.Tasks.Add(todoItem);
            _context.SaveChanges();

            // Act
            todoItem.Description = "Updated Description";
            _repository.Update(todoItem);
            _context.SaveChanges();

            // Assert
            var updatedItem = _context.Tasks.Find(todoItem.Id);
            Assert.NotNull(updatedItem);
            Assert.Equal("Updated Description", updatedItem.Description);
        }

        [Fact]
        public void Delete_ShouldRemoveTask()
        {
            // Arrange
            var todoItem = new Models.Entities.Task
            {
                Description = "Will be deleted",
                Status = TaskStatusEnum.Pending,
                UserId = Guid.NewGuid()
            };

            _context.Tasks.Add(todoItem);
            _context.SaveChanges();

            // Act
            _repository.Delete(todoItem);
            _context.SaveChanges();

            // Assert
            var deletedItem = _context.Tasks.Find(todoItem.Id);
            Assert.Null(deletedItem);
        }

        [Fact]
        public void Query_ShouldReturnAllTasks()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var item1 = new Models.Entities.Task
            {
                Description = "Desc 1",
                Status = TaskStatusEnum.Pending,
                UserId = userId
            };

            var item2 = new Models.Entities.Task
            {
                Description = "Desc 2",
                Status = TaskStatusEnum.InProgress,
                UserId = userId
            };

            _context.Tasks.AddRange(item1, item2);
            _context.SaveChanges();

            // Act
            var results = _repository.Query().ToList();

            // Assert
            Assert.True(results.Count >= 2);
        }

        [Fact]
        public void Query_WithFilter_ShouldReturnFilteredTasks()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var item1 = new Models.Entities.Task
            {
                Description = "Pending Task",
                Status = TaskStatusEnum.Pending,
                UserId = userId
            };

            var item2 = new Models.Entities.Task
            {
                Description = "In Progress Task",
                Status = TaskStatusEnum.InProgress,
                UserId = userId
            };

            _context.Tasks.AddRange(item1, item2);
            _context.SaveChanges();

            var filter = new FilterBy<Models.Entities.Task>(t => t.Status == TaskStatusEnum.Pending);

            // Act
            var results = _repository.Query(filter).ToList();

            // Assert
            Assert.Single(results);
            Assert.Equal(TaskStatusEnum.Pending, results[0].Status);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
