using Microsoft.EntityFrameworkCore;
using TaskManager.Context;
using TaskManager.Models.Base.Query;
using TaskManager.Models.Entities;
using TaskManager.Models.Enums;
using TaskManager.Repository;

namespace TaskManager.UnitTests.Repository
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly TaskManagerContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TaskManagerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TaskManagerContext(options);
            _repository = new UserRepository(_context);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateAsync_ShouldAddUserToDatabase()
        {
            // Arrange
            var user = new User
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "hashedpassword",
                Salt = "randomsalt",
                Role = UserRoleEnum.Default
            };

            // Act
            await _repository.CreateAsync(user);
            await _context.SaveChangesAsync();

            // Assert
            var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
            Assert.NotNull(savedUser);
            Assert.Equal("Test User", savedUser.Name);
            Assert.Equal("test@example.com", savedUser.Email);
        }

        [Fact]
        public void Update_ShouldModifyUser()
        {
            // Arrange
            var user = new User
            {
                Name = "Original Name",
                Email = "original@example.com",
                Password = "hashedpassword",
                Salt = "randomsalt",
                Role = UserRoleEnum.Default
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            user.Name = "Updated Name";
            _repository.Update(user);
            _context.SaveChanges();

            // Assert
            var updatedUser = _context.Users.Find(user.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal("Updated Name", updatedUser.Name);
        }

        [Fact]
        public void Delete_ShouldSoftDeleteUser()
        {
            // Arrange
            var user = new User
            {
                Name = "User to Delete",
                Email = "delete@example.com",
                Password = "hashedpassword",
                Salt = "randomsalt",
                Role = UserRoleEnum.Default
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            _repository.Delete(user);
            _context.SaveChanges();

            // Assert
            var deletedUser = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.Id == user.Id);
            Assert.NotNull(deletedUser);
            Assert.True(deletedUser.Deleted); // Soft delete - registro ainda existe mas marcado como deletado
        }

        [Fact]
        public void Query_ShouldReturnUsers()
        {
            // Arrange
            var user1 = new User
            {
                Name = "Active User",
                Email = "active@example.com",
                Password = "hashedpassword",
                Salt = "randomsalt",
                Role = UserRoleEnum.Default,
                Deleted = false
            };
            var user2 = new User
            {
                Name = "Another User",
                Email = "another@example.com",
                Password = "hashedpassword",
                Salt = "randomsalt",
                Role = UserRoleEnum.Default,
                Deleted = false
            };
            _context.Users.AddRange(user1, user2);
            _context.SaveChanges();

            // Act
            var users = _repository.Query().ToList();

            // Assert
            Assert.True(users.Count >= 2);
        }

        [Fact]
        public void Query_WithFilter_ShouldReturnFilteredUsers()
        {
            // Arrange
            _context.Users.AddRange(
                new User { Name = "Admin User", Email = "admin@example.com", Password = "hash", Salt = "salt", Role = UserRoleEnum.Admin },
                new User { Name = "Default User", Email = "user@example.com", Password = "hash", Salt = "salt", Role = UserRoleEnum.Default }
            );
            _context.SaveChanges();

            var filter = new FilterBy<User>(u => u.Role == UserRoleEnum.Admin);

            // Act
            var users = _repository.Query(filter).ToList();

            // Assert
            Assert.Single(users);
            Assert.Equal(UserRoleEnum.Admin, users[0].Role);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetByEmailAsync_ShouldReturnUserWithEmail()
        {
            // Arrange
            var user = new User
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "hashedpassword",
                Salt = "randomsalt",
                Role = UserRoleEnum.Default
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByEmailAsync("test@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test@example.com", result.Email);
            Assert.Equal("Test User", result.Name);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetByEmailAsync_WithNonExistentEmail_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.Null(result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
