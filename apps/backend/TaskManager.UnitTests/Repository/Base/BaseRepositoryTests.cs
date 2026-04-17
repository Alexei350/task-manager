using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Context;
using TaskManager.Models.Base.Entities;
using TaskManager.Models.Base.Query;
using TaskManager.Repository.Base;

namespace TaskManager.UnitTests.Repository.Base;

public class TestEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}

public class TestEntityMap : IEntityTypeConfiguration<TestEntity>
{
    public void Configure(EntityTypeBuilder<TestEntity> builder)
    {
        builder.HasKey(x => x.Id);
    }
}

public class TestContext : TaskManagerContext
{
    public DbSet<TestEntity> TestEntities { get; set; }

    public TestContext(DbContextOptions<TaskManagerContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new TestEntityMap());
    }
}

public class TestRepository : BaseRepository<TestEntity>
{
    public TestRepository(TaskManagerContext context) : base(context)
    {
    }
}

public class BaseRepositoryTests
{
    private readonly TestContext _context;
    private readonly TestRepository _repository;

    public BaseRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TaskManagerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestContext(options);
        _repository = new TestRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddEntity()
    {
        var entity = new TestEntity { Name = "Test" };
        await _repository.CreateAsync(entity);
        await _context.SaveChangesAsync();

        var saved = await _context.Set<TestEntity>().FirstOrDefaultAsync();
        Assert.NotNull(saved);
        Assert.Equal("Test", saved.Name);
        Assert.NotEqual(Guid.Empty, saved.Id);
    }

    [Fact]
    public async Task Update_ShouldUpdateEntity()
    {
        var entity = new TestEntity { Name = "Test" };
        await _repository.CreateAsync(entity);
        await _context.SaveChangesAsync();

        entity.Name = "Updated";
        _repository.Update(entity);
        await _context.SaveChangesAsync();

        var updated = await _context.Set<TestEntity>().FirstOrDefaultAsync();

        var notNull = Assert.IsType<TestEntity>(updated!);
        Assert.Equal("Updated", notNull.Name);
    }

    [Fact]
    public async Task Delete_ShouldRemoveEntity()
    {
        var entity = new TestEntity { Name = "Test" };
        await _repository.CreateAsync(entity);
        await _context.SaveChangesAsync();

        _repository.Delete(entity);
        await _context.SaveChangesAsync();

        var deleted = await _context.Set<TestEntity>().FirstOrDefaultAsync();
        Assert.Null(deleted);
    }

    [Fact]
    public async Task Query_ShouldReturnQueryable()
    {
        var entity = new TestEntity { Name = "Test" };
        await _repository.CreateAsync(entity);
        await _context.SaveChangesAsync();

        var query = _repository.Query();
        var result = await query.FirstOrDefaultAsync();

        var notNull = Assert.IsType<TestEntity>(result!);
        Assert.Equal("Test", notNull.Name);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnEntity_WhenExists()
    {
        var entity = new TestEntity { Name = "Test" };
        await _repository.CreateAsync(entity);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAsync(entity.Id);

        var notNull = Assert.IsType<TestEntity>(result!);
        Assert.Equal(entity.Id, notNull.Id);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _repository.GetAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrue_WhenExists()
    {
        var entity = new TestEntity { Name = "Test" };
        await _repository.CreateAsync(entity);
        await _context.SaveChangesAsync();

        var result = await _repository.AnyAsync(x => x.Name == "Test");

        Assert.True(result);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnFalse_WhenNotExists()
    {
        var result = await _repository.AnyAsync(x => x.Name == "Test");

        Assert.False(result);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCount()
    {
        await _repository.CreateAsync(new TestEntity { Name = "Test1" });
        await _repository.CreateAsync(new TestEntity { Name = "Test2" });
        await _context.SaveChangesAsync();

        var count = await _repository.CountAsync(x => x.Name.StartsWith("Test"));

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task Query_WithFilterBy_ShouldReturnFilteredResults()
    {
        await _repository.CreateAsync(new TestEntity { Name = "Test1" });
        await _repository.CreateAsync(new TestEntity { Name = "Other" });
        await _context.SaveChangesAsync();

        var filter = new FilterBy<TestEntity>(x => x.Name == "Test1");
        var query = _repository.Query(filter);
        var result = await query.ToListAsync();

        Assert.Single(result);
        Assert.Equal("Test1", result[0].Name);
    }

    [Fact]
    public async Task AnyAsync_WithFilterBy_ShouldReturnTrue_WhenExists()
    {
        await _repository.CreateAsync(new TestEntity { Name = "Test" });
        await _context.SaveChangesAsync();

        var filter = new FilterBy<TestEntity>(x => x.Name == "Test");
        var result = await _repository.AnyAsync(filter);

        Assert.True(result);
    }

    [Fact]
    public async Task CountAsync_WithFilterBy_ShouldReturnCount()
    {
        await _repository.CreateAsync(new TestEntity { Name = "Test1" });
        await _repository.CreateAsync(new TestEntity { Name = "Test2" });
        await _context.SaveChangesAsync();

        var filter = new FilterBy<TestEntity>(x => x.Name.StartsWith("Test"));
        var count = await _repository.CountAsync(filter);

        Assert.Equal(2, count);
    }

    [Fact]
    public void Get_ShouldReturnEntity_WhenExists()
    {
        var entity = new TestEntity { Name = "Test" };
        _context.Add(entity);
        _context.SaveChanges();

        var result = _repository.Get(entity.Id);

        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
    }

    [Fact]
    public void Get_ShouldReturnNull_WhenNotExists()
    {
        var result = _repository.Get(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public void GetForUpdate_ShouldReturnTrackedEntity()
    {
        var entity = new TestEntity { Name = "Test" };
        _context.Add(entity);
        _context.SaveChanges();

        var result = _repository.GetForUpdate(entity.Id);

        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(EntityState.Unchanged, _context.Entry(result).State);
    }

    [Fact]
    public async Task GetForUpdateAsync_ShouldReturnTrackedEntity()
    {
        var entity = new TestEntity { Name = "Test" };
        _context.Add(entity);
        await _context.SaveChangesAsync();

        var result = await _repository.GetForUpdateAsync(entity.Id);

        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(EntityState.Unchanged, _context.Entry(result).State);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldReturnTrue()
    {
        await _repository.CreateAsync(new TestEntity { Name = "Test" });
        var result = await _repository.SaveChangesAsync();

        Assert.True(result);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPagedResults()
    {
        // Add test data
        for (int i = 1; i <= 25; i++)
        {
            await _repository.CreateAsync(new TestEntity { Name = $"Test{i}" });
        }
        await _context.SaveChangesAsync();

        // Get first page
        var filter = new FilterBy<TestEntity>(x => x.Name.StartsWith("Test"));
        var result = await _repository.GetPagedAsync(
            filter,
            x => new { x.Id, x.Name },
            x => x.Name,
            1,
            10
        );

        Assert.True(result.Success);
        Assert.Equal(10, result.Data.Count);
        Assert.Equal(10, result.TotalRecords);
    }

    [Fact]
    public async Task GetPagedAsync_WithDefaultPage_ShouldUsePageOne()
    {
        await _repository.CreateAsync(new TestEntity { Name = "Test" });
        await _context.SaveChangesAsync();

        var filter = new FilterBy<TestEntity>(x => true);
        var result = await _repository.GetPagedAsync(
            filter,
            x => x.Name,
            x => x.Name,
            0, // Default to page 1
            10
        );

        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetPagedAsync_WithDefaultPageSize_ShouldUseTwenty()
    {
        await _repository.CreateAsync(new TestEntity { Name = "Test" });
        await _context.SaveChangesAsync();

        var filter = new FilterBy<TestEntity>(x => true);
        var result = await _repository.GetPagedAsync(
            filter,
            x => x.Name,
            x => x.Name,
            1,
            0 // Default to 20
        );

        Assert.Single(result.Data);
    }

    [Fact]
    public async Task Delete_WithSoftDelete_ShouldMarkAsDeleted()
    {
        var entity = new TestEntity { Name = "Test" };
        await _repository.CreateAsync(entity);
        await _context.SaveChangesAsync();

        _repository.Delete(entity);

        // TestEntity extends BaseEntity, not BaseEntitySoft, so it should be removed
        Assert.Equal(EntityState.Deleted, _context.Entry(entity).State);
    }
}
