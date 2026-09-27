using System.Reflection;
using AutoFixture.Kernel;
using Gadema.Core.DependencyTracking;
using Gadema.Core.Utils;

public sealed class DomainPropertyOmitter : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not PropertyInfo prop)
            return new NoSpecimen();

        // 1. Check Attributes first (Highest priority)
        if (IsTargetedByAttribute(prop))
            return new OmitSpecimen();

        // 2. Check Types (Collections, Primitives)
        if (IsTargetedByType(prop.PropertyType))
            return new OmitSpecimen();

        // 3. Check Names (The "Escape Hatch")
        if (IsTargetedByName(prop.Name))
            return new OmitSpecimen();

        return new NoSpecimen();
    }

    private bool IsTargetedByAttribute(PropertyInfo prop)
    {
        // Handle [Fixture(Hint = Omit)]
        var fixtureAttr = prop.GetCustomAttribute<FixtureAttribute>();
        if (fixtureAttr != null && fixtureAttr.Hint == FixtureHintEnum.Omit)
            return true;

        // Handle [ModelDependency]
        return prop.PropertyType.GetCustomAttribute<ModelDependencyAttribute>() != null;
    }

    private bool IsTargetedByType(Type type)
    {
        // Handle Collections/IEnumerable of Classes
        if (IsEntityCollection(type)) return true;

        // Handle GUIDs (If you want to prevent AutoFixture from randomizing them)
        if (type == typeof(Guid) || type == typeof(Guid?))
          return true;

        return false;
    }

    private bool IsTargetedByName(string name)
    {
        // This is where your "IsDeleted" fix lives. 
        // It's cleaner to have a list of names that should respect defaults.
        return name switch
        {
            "IsDeleted" => true,
            _ => false
        };
    }

    private static bool IsEntityCollection(Type type) =>
        type.IsGenericType &&
        (type.GetGenericTypeDefinition() == typeof(ICollection<>) ||
         type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) &&
        type.GetGenericArguments()[0].IsClass;
}