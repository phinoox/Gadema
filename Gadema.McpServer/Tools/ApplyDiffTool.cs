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
    public string Description => "Applies a hunk (diff) to a file by matching the original content block. Line Numbers are not needed";

    public Dictionary<string, string> GetParameterDescriptions() => new()
    {
        { "file_path", "The path to the target file." },
        { "hunk", "A unified diff hunk. Format: lines of context, '-' for removed, '+' for added." }
    };

    public async Task<string> ExecuteAsync(Dictionary<string, string> args)
    {
        if (!args.TryGetValue("file_path", out var filePath) || !args.TryGetValue("hunk", out var hunkText))
            return "Error: Missing 'file_path' or 'hunk' arguments.";

        if (!File.Exists(filePath))
            return $"Error: File not found at {filePath}";

        try
        {
            if(string.IsNullOrWhiteSpace(hunkText))
                return "Error: Hunk content is empty or invalid.";
            hunkText.Replace("\r\n", "\n");
            // 1. Parse the hunk into lines, ignoring the diff markers (+/-) and metadata (@@)
            var parsedHunk = ParseHunk(hunkText);
            if (parsedHunk == null)
                return "Error: Hunk content is empty or invalid.";

            string currentFileContent = await File.ReadAllTextAsync(filePath);

            // 2. Normalize file content for comparison: replace all \r\n with \n
            string normalizedFileContent = currentFileContent.Replace("\r\n", "\n");
            
            // 3. Build the search string from original lines and the replacement string from final lines
            string searchPattern = string.Join("\n", parsedHunk.OriginalLines);
            string replacementContent = string.Join("\n", parsedHunk.FinalLines);

            // 4. Perform the replacement on the normalized content
            if (normalizedFileContent.Contains(searchPattern))
            {
                string updatedContent = normalizedFileContent.Replace(searchPattern, replacementContent);
                
                // 5. Restore original line endings if necessary or just write with standard \n/ \r\n
                // For simplicity in this example, we'll use the system default for writing back
                await File.WriteAllTextAsync(filePath, updatedContent);
                return "Success: Hunk applied successfully.";
            }
            else
            {
                // Debugging info to help user see what went wrong
                return $"Error: Context mismatch. Could not find the following block in the file:\n\n{searchPattern}";
                
                
            }
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