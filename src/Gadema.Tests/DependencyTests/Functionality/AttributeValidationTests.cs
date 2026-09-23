using System.Reflection;
using Gadema.Core.DependencyTracking;
using Gadema.Core.Models.Access;
using Gadema.Tests.Factory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gadema.Tests.DependencyTests.Functionality;

public class AttributeValidationTests : IClassFixture<ApiWebApplicationFactory>
{
    [Fact]
    public void AllModels_ShouldHaveModelDependencyAttribute()
    {
        var assembly = typeof(ModelDependencyAttribute).Assembly;
        var modelsAssembly = typeof(User).Assembly;

        // Get all classes in Gadema.Core.Models namespace
        var modelTypes = modelsAssembly.GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Namespace?.StartsWith("Gadema.Core.Models") == true)
            .ToList();

        // Get all types with ModelDependency attribute
        var attributedTypes = new HashSet<Type>();
        foreach (var type in assembly.GetExportedTypes())
        {
            var attrs = type.GetCustomAttributes(typeof(ModelDependencyAttribute), false);
            if (attrs.Length > 0)
                attributedTypes.Add(type);
        }

        // Every model should have the attribute (except RootMarker itself)
        var missing = modelTypes.Where(t => !attributedTypes.Contains(t) && t.Name != "RootMarker").ToList();

        Assert.True(missing.Count == 0, $"Missing [ModelDependency] attribute on: {string.Join(", ", missing.Select(t => t.Name))}");
    }
}
