using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gadema.McpServer.Core;
using System.Text.RegularExpressions;

namespace Gadema.McpServer.Tools;

public class ApplyDiffTool : IMcpTool
{
    public string Name => "apply_diff";
    public string Description => "Applies a (diff) to a file . Use this as more safe replacement for edit_existing_file. Use only the MINIMUM number of unique lines required to identify the insertion/replacement point(can also be just 1-2 lines).try to avoid whitespaces/empty lines. Lines need to be a continous unbroken sequence. no header needed and other marks needed. example: {\\n-    public int id;\\n+    public Guid id;\\n}";

    public Dictionary<string, string> GetParameterDescriptions() => new()
    {
        { "file_path", "The path to the target file." },
        { "hunk", "A unified diff hunk. Format: lines of context, '-' for removed, '+' for added with a @@ @@ header." }
    };

    public async Task<string> ExecuteAsync(Dictionary<string, string> args)
    {
        if (!args.TryGetValue("file_path", out var filePath) || !args.TryGetValue("hunk", out var hunkText))
            return "Error: Missing 'file_path' or 'hunk' arguments.";

        if (!File.Exists(filePath))
            return $"Error: File not found at {filePath}";

        try
        {
            if (string.IsNullOrWhiteSpace(hunkText))
                return "Error: Hunk content is empty or invalid.";
            hunkText.Replace("\r\n", "\n");


            // 1. Parse the hunk into lines, ignoring the diff markers (+/-) and metadata (@@)
            var Hunkparts = Regex.Split(hunkText,"(@@.*@@)").ToList<string>();

            if (Hunkparts.Count == 0)
            {
                Hunkparts.Add(hunkText); //in case we dont have @@ seperations
            }

            string currentFileContent = await File.ReadAllTextAsync(filePath);
            string finalContent = currentFileContent.Replace("\r\n", "\n");

            foreach (var hunk in Hunkparts)
            {
                if(string.IsNullOrWhiteSpace(hunk) || hunk.StartsWith("@@"))
                    continue;
                var parsedHunk = ParseHunk(hunk);
                if (parsedHunk == null)
                    return $"Error: Hunk content is empty or invalid.";

                // 3. Build the search string from original lines and the replacement string from final lines
                string searchPattern = string.Join("\n", parsedHunk.OriginalLines);
                string replacementContent = string.Join("\n", parsedHunk.FinalLines);

                // 4. Perform the replacement on the normalized content
                if (finalContent.Contains(searchPattern))
                {
                    finalContent = finalContent.Replace(searchPattern, replacementContent);
                }
                else
                {
                    // Debugging info to help user see what went wrong
                    return $"Error: Context mismatch. Could not find the following block in the file:\n\n{searchPattern}";
                }
            }

            finalContent = finalContent.Replace("\n",Environment.NewLine);
            await File.WriteAllTextAsync(filePath, finalContent);
            return "Success: Hunk applied successfully.";
        }
        catch (Exception ex)
        {
            return $"Error during execution: {ex.Message}";
        }


    }

    private HunkResult ParseHunk(string hunkText)
    {
        var lines = hunkText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var originalLines = new List<string>();
        var finalLines = new List<string>();

        bool hasContent = false;

        foreach (var line in lines)
        {
            // Skip hunk headers
            if (line.StartsWith("@@") || line.StartsWith("---") || line.StartsWith("+++"))
                continue;

            hasContent = true;

            if (line.StartsWith("+"))
            {
                finalLines.Add(line[1..]); // Content after '+'
                // Note: we don't add to originalLines here
            }
            else if (line.StartsWith("-"))
            {
                originalLines.Add(line[1..]); // Content after '-'
                // Note: we don't add to finalLines here
            }
            else
            {
                // Context line: strip the single leading space used by diff tools
                string contextLine = line.Length > 0 && line[0] == ' ' ? line[1..] : line;
                originalLines.Add(contextLine);
                finalLines.Add(contextLine);
            }
        }

        if (!hasContent) return null;

        return new HunkResult { OriginalLines = originalLines, FinalLines = finalLines };
    }

    private class HunkResult
    {
        public List<string> OriginalLines { get; set; }
        public List<string> FinalLines { get; set; }
    }
}