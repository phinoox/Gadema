using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gadema.McpServer.Core;
using Microsoft.CodeAnalysis;

namespace Gadema.McpServer.Tools;

public class BuildTool : IMcpTool
{
    private string _nsPattern = @"(namespace.*;)";
    private string _projectsLocation = "src/Gadema";
    private string _logLocation = "logs";
    public string Name => "build_project";
    public string Description => "this tool builds a project and logs the output to logs/build_[project].log";

    public Dictionary<string, string> GetParameterDescriptions() => new()
    {
        { "project", "the project name. Must be either Core,Data,Api,Test,MockData or WebApp" },
    };

    public async Task<string> ExecuteAsync(Dictionary<string, string> args)
    {


        if (!args.TryGetValue("project", out var project)) return "Error: Missing 'project' argument.";
        project = project.ToLower();

        var projectPath = project switch
        {
            "core" => "src/Gadema.Core/Gadema.Core.csproj",
            "data" => "src/Gadema.Data/Gadema.Data.csproj",
            "api" => "src/Gadema.Api/Gadema.Api.csproj",
            "tests" => "src/Gadema.Tests/Gadema.Tests.csproj",
            "webapp" => "src/Gadema.WebApp/Gadema.WebApp.csproj",
            "mockdata" => "src/Gadema.MockData/Gadema.MockData.csproj",
            _ => ""
        };

        if (string.IsNullOrEmpty(projectPath))
            return "Error: projectname was neither Core,Data,Api,Test,MockData or WebApp";
        string processArgs = $" build {projectPath}";
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = processArgs,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            },
            EnableRaisingEvents = true
        };

        // Start process
        process.Start();

        try
        {

            CancellationToken cancellationToken = default;
            // Wait for exit or cancellation/timeout
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            int timeoutMs = 600000;
            cts.CancelAfter(timeoutMs);

            await process.WaitForExitAsync(cts.Token).ConfigureAwait(false);

        }
        catch (OperationCanceledException)
        {
            process.Kill();
            return "Error: build process timedout";
        }

        // Read outputs if needed
        string output = await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
        string error = await process.StandardError.ReadToEndAsync().ConfigureAwait(false);
        
        string logPath = $"logs/build_{project}.log";
        try
        {

            await File.WriteAllTextAsync(logPath, output);
        }
        catch (Exception e)
        {
            Logger.LogError($"could not write log file {logPath}:{e.Message}");
        }

        if(!string.IsNullOrEmpty(error))
            return $"Error building project:{error}";
        else if(output.Contains("error CS"))
            return $"Error building project: please read {logPath} for more information";
        return $"Successfully build {project}";
    }
}
