using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.CommandLine; // The official library
using Gadema.McpServer;

namespace Gadema.McpServer;

public class CliHandler
{
    private readonly McpServer _server;

    public CliHandler(McpServer server)
    {
        _server = server;
        _server.DiscoverTools();
    }

    public async Task RunAsync(string[] args)
    {
        var rootCommand = new RootCommand("Gadema MCP Server CLI");

        // 1. Discover all tools from the registry
        
        var discoveredTools = _server.GetDiscoveredTools().ToList();

        if (discoveredTools.Count == 0)
        {
            Console.WriteLine("No tools discovered. Check your tool implementations.");
            return;
        }

        // 2. Dynamically build commands for each tool found
        foreach (var tool in discoveredTools)
        {
            var command = new Command(tool.Name, tool.Description);

            // Add options based on the tool's own metadata
            foreach (var param in tool.GetParameterDescriptions())
            {
                var option = new Option<string>($"--{param.Key}", param.Value);
                command.AddOption(option);
            }

            // Set up a generic handler that extracts arguments from the ParseResult
            command.SetHandler(async (context) => 
            {
                var dict = new Dictionary<string, string>();
                foreach (var option in command.Options)
                {
                    // Retrieve the value provided by the user via the parsed context
                    var value = context.ParseResult.GetValueForOption(option);
                    if (value != null)
                    {
                        dict[option.Name] = value.ToString() ?? "";
                    }
                }

                await ExecuteTool(tool.Name, dict);
            });

            rootCommand.AddCommand(command);
        }

        // 3. Invoke the parser with the provided args
        await rootCommand.InvokeAsync(args);
    }

    private async Task ExecuteTool(string commandName, Dictionary<string, string> args)
    {
        var tool = _server.GetTool(commandName);
        if (tool != null)
        {
            try
            {
                Console.WriteLine(await tool.ExecuteAsync(args));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing tool: {ex.Message}");
            }
        }
        else
        {
            // This should technically be unreachable due to the command registration logic
            Console.WriteLine($"Unknown command: {commandName}.");
        }
    }
}