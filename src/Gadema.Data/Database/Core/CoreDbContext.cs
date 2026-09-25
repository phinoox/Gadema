using Microsoft.EntityFrameworkCore;
using Gadema.Core.Models.Base.Projects;
using Gadema.Core.Models.Base.Infrastructure;
using Gadema.Core.Models.Access;
using Gadema.Core.Models.Base.MetaInfo;

namespace Gadema.Data.Database.Core;

/// <summary>
/// The foundational context for the GaDeMa universe.
/// Contains only cross-cutting infrastructure entities that all modules rely on.
/// </summary>
public class CoreDbContext : GademaBaseContext
{
    public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options) { }

    // The "Skeleton" of the system
    public DbSet<Project> Projects { get; set; }
    
    // The "Anchors" (MetaInfo) - every entity in the system points to one of these.
    // We keep them here because they are the universal entry point.
    public DbSet<ContentMetaInfo> MetaInfos { get; set; }

    // Cross-cutting attachments and references
    public DbSet<MediaAttachment> MediaAttachments { get; set; }
    public DbSet<ExternalReference> ExternalReferences { get; set; }
    public DbSet<UserProviderLink> UserProviderLinks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply core-level configurations (e.g., Global Query Filters for Project)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreDbContext).Assembly);
    }
}
