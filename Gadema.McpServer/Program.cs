using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gadema.McpServer;

// --- 1. The Contract (Standardized for the SDK) ---

public interface IMcpTool
{
    string Name { get; }
    string Description { get; }
    Dictionary<string, string> GetParameterDescriptions();
    Task<string> ExecuteAsync(Dictionary<string, string> args);
}

// --- 2. The MCP Server Engine (Using the SDK patterns) ---



public record McpRequest(
    [property: JsonPropertyName("jsonrpc")] string JsonRpc,
    [property: JsonPropertyName("id")] object Id,
    [property: JsonPropertyName("method")] string Method,
    [property: JsonPropertyName("params")] JsonElement Params
);

// --- 3. Entry Point ---

public class Program
{
    public static async Task Main(string[] args)
    {
            var server = new McpServer();

        if (args.Length > 0 && args[0] == "--cli")
        {
            Console.WriteLine("runnin mcp in cli mode");
            var handler = new CliHandler(server);
            await handler.RunAsync(args.Skip(1).ToArray());
    }
        else
        {
            Console.WriteLine("Starting server..."); 
            await server.RunAsync();
            Console.Error.WriteLine("Starting server..."); 
        }
    }
}

