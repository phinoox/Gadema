using FluentAssertions;
using Gadema.Core.Models.Base.MetaInfo;
using Gadema.Core.Models.Game.Attributes;
using Gadema.Core.Models.Writing.Narrative;
using Gadema.Data.Database;
using Gadema.Tests.Factory;
using Gadema.Tests.Helpers;
using Gadema.Tests.Seeders;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gadema.Tests.DependencyTests.Functionality;
public class SystemSetupTests : IClassFixture<ApiWebApplicationFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    public SystemSetupTests(ApiWebApplicationFactory factory)
    {
        _scope = factory.Services.CreateScope();
    }

    public void Dispose() => _scope.Dispose();

    [Fact]
    public void Bootstrap_ShouldPopulateAllBuiltInTags()
    {
        // 1. Seed the fundamental "vocabulary" for each module
        DbSeeder.SeedEnumTags<BaseTag>(_scope);
        DbSeeder.SeedEnumTags<WritingTag>(_scope);
        DbSeeder.SeedEnumTags<GameTag>(_scope);

        // 2. Verify that they actually exist in the DB
        var db = _scope.ServiceProvider.GetRequiredService<GameDbContext>();
        
        // Check a few samples from different modules
        db.Set<MetaTag>().Any(t => t.Name == "Fantasy").Should().BeTrue();
        db.Set<MetaTag>().Any(t => t.Name == "Fire").Should().BeTrue();
        db.Set<MetaTag>().Any(t => t.Name == "Tutorial").Should().BeTrue();
    }
}