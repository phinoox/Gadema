using System.Diagnostics;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using Gadema.Data.Database;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

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
                if( IsEntityCollection(prop.PropertyType))
                    return new OmitSpecimen();
                
                var FixtureAttribute = prop.GetCustomAttribute<FixtureAttribute>();
                var hint = FixtureAttribute == null ? FixtureHintEnum.None : FixtureAttribute.Hint;
                if(hint == FixtureHintEnum.Omit)
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
            // Get the caller frame (index 0 is current, index 1 is caller)
            do
            {
                callerFrame = stackTrace.GetFrame(index);
                factAttrib = callerFrame.GetMethod().GetCustomAttribute(typeof(FactAttribute));
                index++;

            } while (factAttrib == null);

            // Retrieve the line number
            string methodName = callerFrame.GetMethod().Name;
            int lineNumber = callerFrame.GetFileLineNumber();
            Console.WriteLine($" SeedError in {methodName} line {lineNumber} : Could not seed entity of type {typeof(T)}");
            throw;
        }
        return entity;
    }

    /// <summary>Create an instance without persisting (for overriding values first).</summary>
    public static T Create<T>() where T : class => _fixture.Create<T>();

    /// <summary>Create with specific overrides, e.g. For&lt;ContentItem&gt;(x => x.ProjectId, projectId).</summary>
    public static T Create<T>(Action<T>? customize = null) where T : class
    {
        var instance = _fixture.Create<T>();   // all settable properties filled with sensible defaults
        customize?.Invoke(instance);           // caller overrides what it needs (FKs etc.)
        return instance;
    }
}