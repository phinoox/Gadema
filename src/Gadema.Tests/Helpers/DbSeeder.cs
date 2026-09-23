using System.Diagnostics;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;

using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Microsoft.Data.Sqlite;
using Gadema.Core.DependencyTracking;
using Gadema.Core.Utils;
using Gadema.Core.Models.Base.MetaInfo;

namespace Gadema.Tests.Seeders;
/// <summary>
/// Generic DB seeding for any mapped entity.
/// Instance generation (required columns filled) is delegated to AutoFixture,
/// so adding a new model or column never requires touching seeder code.
/// </summary>
public static class DbSeeder
{
    private static readonly Fixture _fixture = CreateFixture();

    private static Fixture CreateFixture()
    {
        var fixture = new Fixture();

        // Models contain circular navigations (e.g. Team ↔ TeamMember).
        // Instead of throwing, stop recursing and leave the cycle unset.
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // Leave collection navigations (ICollection<T> where T is a class) empty:
        // EF would cascade-insert the random unattached instances and hit key conflicts.
        // Scalar FKs are still filled — only dependent-entity collections stay unset.
        fixture.Customizations.Add(new EntityCollectionOmitter());

        return fixture;
    }

    private sealed class EntityCollectionOmitter : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is PropertyInfo prop)
            {
                if (prop.PropertyType == typeof(Guid) || prop.PropertyType == typeof(Guid?))
                    return new OmitSpecimen();

                if (IsEntityCollection(prop.PropertyType))
                    return new OmitSpecimen();

                var FixtureAttribute = prop.GetCustomAttribute<FixtureAttribute>();
                var hint = FixtureAttribute == null ? FixtureHintEnum.None : FixtureAttribute.Hint;
                if (hint == FixtureHintEnum.Omit)
                    return new OmitSpecimen();

                var dependencyAttribute = prop.PropertyType.GetCustomAttribute<ModelDependencyAttribute>();
                if(dependencyAttribute != null)
                    return new OmitSpecimen();

            }

            return new NoSpecimen();
        }

        private static bool IsEntityCollection(Type type) =>
            type.IsGenericType &&
            (
            type.GetGenericTypeDefinition() == typeof(ICollection<>) ||
            type.GetGenericTypeDefinition() == typeof(IEnumerable<>)

            ) &&
            type.GetGenericArguments()[0].IsClass;
    }


    // Shared context to track seeded instances by type during a single AutoSeed call
    private static readonly AsyncLocal<Dictionary<Type, Guid?>> _seededCache = new();
    private static readonly AsyncLocal<Dictionary<Type, object?>> _seededObjects = new();


    public static void ResetSeedingCache()
    {
        if (_seededCache.Value == null)
            _seededCache.Value = new Dictionary<Type, Guid?>();
        else
            _seededCache.Value!.Clear();
    }

    /// <summary>
    /// Seeds all members of an enum into the MetaTag table using deterministic GUIDs.
    /// </summary>
    public static void SeedEnumTags<T>(IServiceScope scope) where T : struct,Enum
    {
        var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        var type = typeof(T);

        // 1. Get the ModuleIndex from the attribute on the Enum
        var attr = type.GetCustomAttribute<ModuleIndexAttribute>();
        if (attr == null)
            throw new InvalidOperationException($"Enum {type.Name} is missing the [ModuleIndex] attribute.");

        int moduleIndex = attr.Index;

        
        // 2. Iterate through all enum members
        foreach (var value in Enum.GetValues<T>())
        {

            int intValue = Convert.ToInt32(value); // Safe conversion for enums
            var tagId = ReservedGuid.Create(moduleIndex, intValue);
            string tagName = value.ToString();

            // 3. Check if it already exists to prevent duplicates on re-runs
            if (!db.Set<MetaTag>().Any(t => t.Id == tagId))
            {
                db.Set<MetaTag>().Add(new MetaTag
                {
                    Id = tagId,
                    Name = tagName,
                    Slug = StringSanitizer.Normalize(tagName) // Ensure web-friendly slug
                });
            }
        }

        db.SaveChanges();
    }

    /// <summary>
    /// Seeds a single instance into the database. Auto-resolves FK dependencies first.
    /// </summary>
    public static T AutoSeed<T>(IServiceScope scope,  string? domainNamespace = null, Action<T>? customize = null) where T : class
    {
        var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();

         var targetType = typeof(T);
         var assembly = targetType.Assembly;

        // Initialize cache for this seed call
        if (_seededCache.Value == null)
            _seededCache.Value = new Dictionary<Type, Guid?>();
        if(_seededObjects.Value == null)
            _seededObjects.Value = new Dictionary<Type, object?>();

        // Step 1: Get topological order
        var sortedTypes = DependencyResolver.GetRequiredAncestors(assembly, targetType);
      //  var sortedTypes = DependencyResolver.ResolveDependencies(typeof(T).Assembly,domainNamespace).SortedTypes;
        if (sortedTypes == null) return default;

        int index = sortedTypes.IndexOf(typeof(T));
        if (index < 0) throw new InvalidOperationException($"Type {typeof(T).Name} not found in dependency graph");

        // Step 2: Seed all parents before this type
        for (int i = 0; i < index; i++)
        {
            var parentType = sortedTypes[i];
            if (_seededCache.Value.Keys.Contains(parentType))
                continue;
            var parentAttrs = DependencyResolver.GetGraph((parentType).Assembly)[parentType];
            if (parentAttrs == null || parentAttrs.Count == 0) continue;

            var autoSeedMethod = typeof(DbSeeder)
     .GetMethods(BindingFlags.Public | BindingFlags.Static)
     .FirstOrDefault(m => m.Name == nameof(AutoSeed) && m.IsGenericMethod);

            if (autoSeedMethod == null)
            {
                throw new InvalidOperationException($"Could not find the generic method '{nameof(AutoSeed)}' on {nameof(DbSeeder)}.");
            }

            var closedMethod = autoSeedMethod.MakeGenericMethod(parentType);
            try

            {
                closedMethod.Invoke(null, new object[] { scope, null,(Action<object>?)null });
            }
            catch (System.Exception)
            {
                Console.WriteLine($"Failed to invoke {closedMethod.Name} for type {parentType.Name}");
                throw;
            }


        }

        // Step 3: Create, customize, persist
        T? instance;
        if (customize != null)
        {
            instance = CreateInstance<T>(typeof(T));
            customize(instance); // customize is now Action<T> — works correctly
        }
        else
        {
            instance = _fixture.Create<T>();
        }

        // Wire FK properties from already-seeded parents
        SetFKProperties(db, typeof(T), instance, scope);


        db.Set<T>().Add(instance);
        try
        {
            db.SaveChanges();
        }
        catch (SqliteException e)
        {
            Console.WriteLine($"SQLite exception when trying to save entity {typeof(T).Name}:{e.Message}");
            throw;
        }
        // Cache the seeded ID
        var id = instance.GetType().GetProperty("Id")?.GetValue(instance) as Guid?;
        if (id != null){
            _seededCache.Value[typeof(T)] = id;
            _seededObjects.Value[typeof(T)] = instance;
        }

        return instance;
    }

    private static void SetFKProperties(DbContext db, Type targetType, object instance, IServiceScope scope)
    {
        if (_seededCache.Value == null) return;
        // Get the graph for this type's assembly
        var graph = DependencyResolver.GetGraph(targetType.Assembly);
        if (graph == null || !graph.ContainsKey(targetType)) return;

        // Get the dependencies of the target type
        var attrs = graph[targetType];
        if (attrs == null || attrs.Count == 0) return;

        foreach (var dependencyTypeAttribute in attrs)
        {
            foreach (var dependencyType in dependencyTypeAttribute.DependentTypes)
            {
                // Check if the dependency has been seeded
                if (!_seededCache.Value.TryGetValue(dependencyType, out var depId) || depId == null)
                    continue;

                // Set the FK property: "{DependencyTypeName}Id" → {dependencyId}
                var fkPropertyName = $"{dependencyType.Name}Id";
                var propInfo = targetType.GetProperty(fkPropertyName, BindingFlags.Public | BindingFlags.Instance);

                if (propInfo != null && propInfo.CanWrite)
                    propInfo.SetValue(instance, depId.Value);

                if (!_seededObjects.Value.TryGetValue(dependencyType, out var depObject) || depObject == null)
                    continue;

                 var navPropertyName = $"{dependencyType.Name}";
                var navPropInfo = targetType.GetProperty(navPropertyName, BindingFlags.Public | BindingFlags.Instance);

                if (navPropInfo != null && navPropInfo.CanWrite)
                    navPropInfo.SetValue(instance, depObject);
            }
        }
    }

    /// <summary>
    /// Creates an instance without persisting (for pre-built overrides).
    /// </summary>
    public static T Create<T>(Action<T>? customize = null) where T : class
    {
        var instance = _fixture.Create<T>() ?? throw new InvalidOperationException($"No parameterless constructor on {typeof(T).FullName}");
        customize?.Invoke(instance);
        return instance;
    }

    /// <summary>Create + persist an entity with sensible defaults; return it (use .Id).</summary>
    public static T Seed<T>(IServiceScope scope) where T : class
        => Seed(scope, Create<T>());

    /// <summary>Persist a pre-built instance.</summary>
    public static T Seed<T>(IServiceScope scope, T entity) where T : class
    {
        var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        db.Set<T>().Add(entity);
        try
        {
            db.SaveChanges();
        }
        catch (Exception ex)
        {
            StackTrace stackTrace = new StackTrace(true);
            int index = 1;
            Attribute factAttrib = null;
            StackFrame callerFrame;
            do
            {
                callerFrame = stackTrace.GetFrame(index);
                factAttrib = callerFrame.GetMethod().GetCustomAttribute(typeof(FactAttribute));
                index++;
            } while (factAttrib == null);
            db.Database.ExecuteSqlRaw("PRAGMA foreign_key_check;");
            string methodName = callerFrame.GetMethod().Name;
            int lineNumber = callerFrame.GetFileLineNumber();
            Console.WriteLine($" SeedError in {methodName} line {lineNumber} : Could not seed entity of type {typeof(T)}");
            throw;
        }
        return entity;
    }

    private static T? CreateInstance<T>(Type t) where T : class
    {
        object instancedObject = t.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null)?
        .Invoke(Array.Empty<object>()) ?? Activator.CreateInstance(t)!;
        return instancedObject == null ? default(T) : (T)instancedObject;
    }


    private static Guid GetSeedId(Type type)
    {
        // This would need to be passed in from the recursive Seed call — 
        // for now we return a placeholder. In production, pass the seeded instance through context.
        throw new NotImplementedException("Pass parent instances via IServiceScope context.");
    }
}