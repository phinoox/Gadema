using System;

namespace Gadema.Core.DependencyResolver;

/// <summary>
/// Declares that a model depends on another model's existence as an FK reference.
/// Used by DbSeeder to compute the topological sort order for database seeding.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ModelDependencyAttribute : Attribute
{
    public Type[] DependentTypes { get; }

    public ModelDependencyAttribute(params Type[] dependentTypes) =>
        DependentTypes = dependentTypes ?? Array.Empty<Type>();
}

