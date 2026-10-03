using System;
using System.Reflection;
using System.Linq;

namespace Gadema.MockData.Utils;

/// <summary>
/// Provides reflection-based utilities for synchronizing data between DTOs and entities.
/// </summary>
public static class ReflectionMapper
{
    /// <summary>
    /// Applies non-default values from a source object to a target object via reflection.
    /// </summary>
    /// <typeparam name="TTarget">The type of the object being updated.</typeparam>
    /// <typeparam name="TSource">The type of the source DTO.</typeparam>
    /// <param name="target">The instance to update.</param>
    /// <param name="source">The source containing new values.</param>
    public static void ApplyUpdate<TTarget, TSource>(TTarget target, TSource source)
        where TTarget : class
        where TSource : class
    {
        if (target == null || source == null) return;

        var sourceType = typeof(TSource);
        var targetType = typeof(TTarget);

        // Get all public instance properties of the source
        var sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var sourceProp in sourceProperties)
        {
            // 1. Find a matching property on the target by name
            var targetProp = targetType.GetProperty(sourceProp.Name, BindingFlags.Public | BindingFlags.Instance);

            if (targetProp != null && targetProp.CanWrite)
            {
                // 2. Get the value from the source
                object? value = sourceProp.GetValue(source);

                // 3. Only apply if the value is not the default for its type
                if (value != null && !IsDefaultValue(value, sourceProp.PropertyType))
                {
                    // 4. Perform Type Safety Check: Is target property assignable from source property type?
                    if (IsAssignableFrom(targetProp.PropertyType, sourceProp.PropertyType))
                    {
                        targetProp.SetValue(target, value);
                    }
                }
            }
        }
    }

    private static bool IsDefaultValue(object value, Type type)
    {
        // Handle Nullable<T> by getting the underlying type
        Type actualType = Nullable.GetUnderlyingType(type) ?? type;
        return value.Equals(Activator.CreateInstance(actualType));
    }

    private static bool IsAssignableFrom(Type targetType, Type sourceType)
    {
        // Handle Nullable<T> mapping (e.g., int -> int?)
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return IsAssignableFrom(Nullable.GetUnderlyingType(targetType)!, sourceType);
        }

        // Standard assignment check
        return targetType.IsAssignableFrom(sourceType);
    }
}
