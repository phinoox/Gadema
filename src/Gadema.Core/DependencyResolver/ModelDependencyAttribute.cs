using System;

namespace Gadema.Core.DependencyResolver;


// <summary>
/// Marker type indicating a model has no FK dependencies (root of the graph).
/// Used with [ModelDependency(typeof(RootMarker))] on root models.
/// </summary>
public static class RootMarker { }

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

