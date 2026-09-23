using System.Reflection;
using Gadema.Core.Models.Base.MetaInfo;


namespace Gadema.Core.Utils;

[AttributeUsage(AttributeTargets.Enum)]
public class ModuleIndexAttribute(int index) : Attribute
{
    public int Index { get; } = index;
}

public static class EnumGuidExtensions
{
    /// <summary>
    /// Converts an enum member into a deterministic GUID using the [ModuleIndex] attribute.
    /// </summary>
    public static Guid ToGuid<T>(this T value) where T : Enum
    {
        var type = typeof(T);
        var attr = type.GetCustomAttribute<ModuleIndexAttribute>();

        if (attr == null)
            throw new InvalidOperationException($"Type {type.Name} is missing the [ModuleIndex] attribute.");

        // The integer value of the enum acts as our unique serial within the module
        int serial = Convert.ToInt32(value);

        return ReservedGuid.Create(attr.Index, serial);
    }
}