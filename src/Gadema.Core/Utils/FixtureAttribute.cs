[AttributeUsage(AttributeTargets.Property)]
public class FixtureAttribute : Attribute
{
    public FixtureHintEnum Hint { get; }

    public FixtureAttribute(FixtureHintEnum hint)
    {
        Hint = hint;
    }
}

public enum FixtureHintEnum
{
    None,
    Omit
}

