using System;
using System.Collections.Generic;
using System.Linq;

namespace Gadema.McpServer.Core;

public static class DiffEngine
{
    // --- LCS Implementation (The "Read" side) ---
    public static List<DiffLine> ComputeDiff(string[] s1, string[] s2)
    {
        int[,] table = ComputeLcsTable(s1, s2);
        return BacktrackLcs(table, s1, s2);
    }

    private static int[,] ComputeLcsTable(string[] s1, string[] s2)
    {
        int m = s1.Length; int n = s2.Length;
        int[,] table = new int[m + 1, n + 1];
        for (int i = 1; i <= m; i++)
            for (int j = 1; j <= n; j++)
                table[i, j] = s1[i - 1] == s2[j - 1] ? table[i - 1, j - 1] + 1 : Math.Max(table[i - 1, j], table[i, j - 1]);
        return table;
    }

    private static List<DiffLine> BacktrackLcs(int[,] table, string[] s1, string[] s2)
    {
        var result = new List<DiffLine>();
        int i = s1.Length; int j = s2.Length;
        while (i > 0 || j > 0)
        {
            if (i > 0 && j > 0 && s1[i - 1] == s2[j - 1]) { result.Add(new DiffLine(DiffType.Unchanged, i, j, s1[i - 1])); i--; j--; }
            else if (j > 0 && (i == 0 || table[i, j - 1] >= table[i - 1, j])) { result.Add(new DiffLine(DiffType.Added, 0, j, s2[j - 1])); j--; }
            else if (i > 0 && (j == 0 || table[i, j - 1] < table[i - 1, j])) { result.Add(new DiffLine(DiffType.Removed, i, 0, s1[i - 1])); i--; }
        }
        result.Reverse(); return result;
    }

    // --- Hunk Parsing (The "Write" side) ---
    public static (List<string> context, List<string> removed, List<string> added) ParseHunk(string hunkText)
    {
        var lines = hunkText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var context = new List<string>();
        var removed = new List<string>();
        var added = new List<string>();

        foreach (var line in lines)
        {
            if (line.StartsWith("+")) added.Add(line[1..]);
            else if (line.StartsWith("-")) removed.Add(line[1..]);
            else context.Add(line.Length > 1 ? line[1..] : line); 
        }

        return (context, removed, added);
    }
}
