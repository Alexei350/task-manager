using TaskManager.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace TaskManager.Context
{
    public class TaskManagerContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }

        public TaskManagerContext(DbContextOptions<TaskManagerContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
