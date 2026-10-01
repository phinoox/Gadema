namespace Gadema.McpServer.Core;

public enum DiffType { Added, Removed, Unchanged }

public record DiffLine(DiffType Type, int Line1, int Line2, string Text);

public record Block(
    int StartLine1, 
    int EndLine1, 
    int StartLine2, 
    int EndLine2, 
    List<DiffLine> Lines
);
