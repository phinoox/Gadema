using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Gadema.Core.DependencyResolver;

/// <summary>
/// Builds a dependency graph from [ModelDependency] attributes and produces
/// a topologically sorted list of types (FK-order: parents before children).
/// </summary>
// Gadema.Core/Seeders/DependencyResolver.cs (updated)
public static class DependencyResolver
{
    /// <summary>
    /// Scans only Gadema.Core.Models assembly for types with [ModelDependency] attributes.
    /// Returns a tuple containing the topologically sorted types and any cyclic types.
    /// </summary>
    public static (List<Type> SortedTypes, List<Type> CyclicTypes) ResolveDependencies()
    {
        // Explicitly target Gadema.Core.Models — not the current assembly (Gadema.Tests.dll)
        var modelsAssembly = typeof(Gadema.Core.Models.User).Assembly;

        return ResolveDependencies(modelsAssembly);
    }

     /// <summary>
    /// Generic overload for scanning any assembly.
    /// Useful if you want to scan a different project later (e.g., Gadema.Api.Models).
    /// Returns a tuple containing the topologically sorted types and any cyclic types.
    /// </summary>
    public static (List<Type> SortedTypes, List<Type> CyclicTypes) ResolveDependencies(Assembly assembly)
    {
        // ─────────────────────────────────────────────────────────────────────
        // STEP 1: Discover all types with [ModelDependency] attributes
        // ─────────────────────────────────────────────────────────────────────
        var typeMap = new Dictionary<Type, List<ModelDependencyAttribute>>();

        foreach (var type in assembly.GetExportedTypes().Where(t => t.IsClass && !t.IsAbstract))
        {
            var attrs = type.GetCustomAttributes(typeof(ModelDependencyAttribute), false);
            if (attrs.Length == 0) continue;

            var deps = attrs.Cast<ModelDependencyAttribute>()
                .SelectMany(a => a.DependentTypes)
                .Distinct()
                .ToList();

            // Skip types where all dependencies are nullable reference types (no FK to persist yet)
            //var hasNonNullDep = deps.Any(d => !d.IsNullableReferenceType());
            //if (!hasNonNullDep && attrs.Length == 1) continue;

            typeMap[type] = attrs.Cast<ModelDependencyAttribute>().ToList();
        }

        if (typeMap.Count == 0) return (new(), new());

        // ─────────────────────────────────────────────────────────────────────
        // STEP 2: Collect ALL types in the graph (both types with attributes AND their dependencies)
        // Types without [ModelDependency] but referenced as dependencies become "root" nodes.
        // ─────────────────────────────────────────────────────────────────────
        var allTypes = new HashSet<Type>(typeMap.Keys);
        foreach (var kvp in typeMap)
        {
            foreach (var depType in kvp.Value.SelectMany(a => a.DependentTypes))
            {
                if (depType == kvp.Key) continue; // skip self-references
                allTypes.Add(depType);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // STEP 3: Build directed graph (A depends on B → edge A→B, meaning A seeds AFTER B)
        // Only include edges where both endpoints are in the graph.
        // ─────────────────────────────────────────────────────────────────────
        var adjacency = new Dictionary<Type, List<Type>>();
        foreach (var type in allTypes) adjacency[type] = new List<Type>();

        foreach (var kvp in typeMap)
        {
            foreach (var depType in kvp.Value.SelectMany(a => a.DependentTypes).Distinct())
            {
                if (depType == kvp.Key) continue; // skip self-references
                if (allTypes.Contains(depType))
                    adjacency[kvp.Key].Add(depType);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // STEP 4: Topological sort via Kahn's algorithm
        // ─────────────────────────────────────────────────────────────────────
        // Initialize in-degree for each node (count of direct dependencies within the graph)
        var inDegree = new Dictionary<Type, int>();
        foreach (var type in allTypes)
            inDegree[type] = adjacency[type].Count;

        // Enqueue all nodes with zero in-degree (no dependencies within the graph — these are "roots")
        var queue = new Queue<Type>();
        foreach (var kvp in inDegree)
        {
            if (kvp.Value == 0)
                queue.Enqueue(kvp.Key);
        }

        // Process nodes in order: remove edges, enqueue nodes whose in-degree becomes zero
        var sorted = new List<Type>();
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            sorted.Add(current);

            foreach (var kvp in typeMap)
            {
                if (adjacency.TryGetValue(kvp.Key, out var neighbors) && neighbors.Contains(current))
                {
                    inDegree[kvp.Key]--;
                    if (inDegree[kvp.Key] == 0 && !sorted.Contains(kvp.Key))
                        queue.Enqueue(kvp.Key);
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // STEP 5: Cycle detection — unvisited nodes means a cycle exists
        // ─────────────────────────────────────────────────────────────────────
        var cyclicTypes = new List<Type>();
        foreach (var t in allTypes)
        {
            if (!sorted.Contains(t))
            {
                cyclicTypes.Add(t);
                Console.WriteLine($"[DependencyResolver] ⚠ Cycle detected involving: '{t.Name}'");
            }
        }

        Console.WriteLine($"[DependencyResolver] Graph built: {sorted.Count} types in topological order, {cyclicTypes.Count} cyclic.");
        return (sorted, cyclicTypes);
    }


    /// <summary>
    /// Returns a map of Type → list of [ModelDependencyAttribute] for inspection/debugging.
    /// Useful for test assertions on graph correctness.
    /// </summary>
    public static Dictionary<Type, List<ModelDependencyAttribute>> GetGraph(Assembly assembly)
    {
        var typeMap = new Dictionary<Type, List<ModelDependencyAttribute>>();

        foreach (var type in assembly.GetExportedTypes().Where(t => t.IsClass && !t.IsAbstract))
        {
            var attrs = type.GetCustomAttributes(typeof(ModelDependencyAttribute), false);
            if (attrs.Length > 0)
                typeMap[type] = attrs.Cast<ModelDependencyAttribute>().ToList();
        }

        return typeMap;
    }

    private static bool IsNullableReferenceType(this Type t) =>
        typeof(object).IsAssignableFrom(t) && !t.IsValueType;
}