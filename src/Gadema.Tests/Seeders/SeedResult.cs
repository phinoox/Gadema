namespace Gadema.Tests.Seeders;

/// <summary>
/// Result of seeding a single model instance.
/// </summary>
public sealed class SeedResult<T> where T : class
{
    public Guid? Id { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; } = null!;

    public override string ToString() =>
        IsSuccess
            ? $"✓ Seed({typeof(T).Name}, id={Id})"
            : $"✗ Seed({typeof(T).Name}) — {ErrorMessage}";
}