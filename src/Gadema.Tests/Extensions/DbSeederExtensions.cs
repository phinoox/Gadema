
using Gadema.Core.DependencyTracking;
using Gadema.Core.Models;
using Gadema.Core.Models.Access;
using Gadema.Tests.Helpers;
using Gadema.Tests.Seeders;
using Microsoft.Extensions.DependencyInjection;


namespace Gadema.Tests.Extensions;

public static class DbSeederExtensions
{
    public static T Seed<T>(this IServiceScope scope, Action<T>? customize = null) where T : class =>
        DbSeeder.AutoSeed<T>(scope, null,customize);

    public static T Create<T>(this IServiceScope scope, Action<T>? customize = null) where T : class
        => DbSeeder.Create<T>(customize);

    private static Guid? GetSeedId(Type type)
    {
        var sorted = DependencyResolver.ResolveDependencies().SortedTypes;
        if (sorted == null || !sorted.Contains(type)) return null;

        var map = new Dictionary<Type, object?>();
        foreach (var t in sorted)
            if (t != typeof(ProjectMember)) // avoid circular Team↔TeamMember for this pass
                map[t] = Activator.CreateInstance(t);

        return map.TryGetValue(type, out var v) ? ((Guid?)v as Guid?) : null;
    }
}