using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gadema.McpServer.Core;

namespace Gadema.McpServer.Tools;

public class FileDiffTool : IMcpTool
{
    public string Name => "diff_files";
    public string Description => "Compares two files and returns a block-based diff optimized for AI analysis.";

    public Dictionary<string, string> GetParameterDescriptions() => new()
    {
        { "file1", "The path to the first file." },
        { "file2", "The path to the second file." }
    };

    public async Task<string> ExecuteAsync(Dictionary<string, string> args)
    {
        if (!args.TryGetValue("file1", out var f1)) return "Error: Missing 'file1' argument.";
        if (!args.TryGetValue("file2", out var f2)) return "Error: Missing 'file2' argument.";

        if (!File.Exists(f1)) return $"Error: File not found: {f1}";
        if (!File.Exists(f2)) return $"Error: File not found: {f2}";

        string[] lines1 = await File.ReadAllLinesAsync(f1);
        string[] lines2 = await File.ReadAllLinesAsync(f2);

        var allLines = DiffEngine.ComputeDiff(lines1, lines2);
        var blocks = GroupIntoBlocks(allLines, 3);

        var output = $"FILE_DIFF: {f1} -> {f2}\n";
        foreach (var block in blocks)
        {
            int len1 = Math.Max(0, block.EndLine1 - block.StartLine1 + 1);
            int len2 = Math.Max(0, block.EndLine2 - block.StartLine2 + 1);
            output += $"\n@@ -{block.StartLine1},{len1} +{block.StartLine2},{len2} @@\n";

            foreach (var line in block.Lines)
            {
                string prefix = line.Type switch {
                    DiffType.Added => "+ ",
                    DiffType.Removed => "- ",
                    _ => "  "
                };
                output += $"{prefix}{line.Text}\n";
            }
        }
        return output;
    }

    private List<Block> GroupIntoBlocks(List<DiffLine> allLines, int contextSize)
    {
        int n = allLines.Count;
        if (n == 0) return new List<Block>();

        var changeIndices = Enumerable.Range(0, n).Where(i => allLines[i].Type != DiffType.Unchanged).ToList();
        if (changeIndices.Count == 0) return new List<Block>();

        var ranges = changeIndices.Select(idx => (start: Math.Max(0, idx - contextSize), end: Math.Min(n - 1, idx + contextSize))).ToList();
        var merged = new List<(int start, int end)>();
        if (ranges.Count > 0) {
            var curr = ranges[0];
            for (int i = 1; i < ranges.Count; i++) {
                if (ranges[i].start <= curr.end + 1) curr = (curr.start, Math.Max(curr.end, ranges[i].end));
                else { merged.Add(curr); curr = ranges[i]; }
            }
            merged.Add(curr);
        }

        var blocks = new List<Block>();
        foreach (var r in merged) {
            var bLines = allLines.Skip(r.start).Take(r.end - r.start + 1).ToList();
            int minL1 = bLines.Any(l => l.Line1 > 0) ? bLines.Where(l => l.Line1 > 0).Min(l => l.Line1) : 0;
            int maxL1 = bLines.Any(l => l.Line1 > 0) ? bLines.Where(l => l.Line1 > 0).Max(l => l.Line1) : 0;
            int minL2 = bLines.Any(l => l.Line2 > 0) ? bLines.Where(l => l.Line2 > 0).Min(l => l.Line2) : 0;
            int maxL2 = bLines.Any(l => l.Line2 > 0) ? bLines.Where(l => l.Line2 > 0).Max(l => l.Line2) : 0;
            blocks.Add(new Block(minL1, maxL1, minL2, maxL2, bLines));
        }
        return blocks;
    }
}
