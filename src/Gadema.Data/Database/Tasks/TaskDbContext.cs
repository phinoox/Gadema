using Microsoft.EntityFrameworkCore;
using Gadema.Core.Models.Tasks;

namespace Gadema.Data.Database.Tasks;

/// <summary>
/// Manages task-related data, including project tasks and reviews.
/// </summary>
public class TaskDbContext : GademaBaseContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }

    // Tasks ownership
    public DbSet<ProjectTask> ProjectTasks { get; set; }
    public DbSet<ProjectTaskComment> ProjectTaskComments { get; set; }
    public DbSet<ReviewStatus> ReviewStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations specific to the Tasks module
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskDbContext).Assembly);
    }
}
