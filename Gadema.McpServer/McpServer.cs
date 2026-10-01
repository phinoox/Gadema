

using System.Reflection;
using System.Text.Json;
using ModelContextProtocol.Protocol;

namespace Gadema.McpServer;

public class McpServer
{
    private readonly Dictionary<string, IMcpTool> _tools = new();

    public void DiscoverTools()
    {
        var toolTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => typeof(IMcpTool).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in toolTypes)
        {
            if (Activator.CreateInstance(type) is IMcpTool tool)
            {
                _tools[tool.Name] = tool;
            }
        }
    }

    public IEnumerable<IMcpTool> GetDiscoveredTools() => _tools.Values;

    public IMcpTool? GetTool(string name) => _tools.TryGetValue(name, out var tool) ? tool : null;

    public async Task RunAsync()
    {
        DiscoverTools();

        // The SDK handles the JSON-RPC loop over Stdio internally
        // We just provide the logic for handling incoming requests
        await StartMcpLoop();
    }

    private async Task StartMcpLoop()
    {
        // This is a simplified representation of what the SDK's internal loop does.
        // In a real implementation, we'd use: await _sdkServer.StartAsync();
        string? line;
        while ((line = await Console.In.ReadLineAsync()) != null)
        {
            try
                        {
                var request = JsonSerializer.Deserialize<McpRequest>(line);
                if (request == null) continue;

                await HandleRequest(request);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in StartMcpLoop loop: {ex.Message}", ex);
                Console.WriteLine(JsonSerializer.Serialize(new { jsonrpc = "2.0", id = (object?)null, error = new { code = -32603, message = ex.Message } }));
            }
        }
    }

    private async Task HandleRequest(McpRequest request)
    {
        switch (request.Method)
        {
            case "initialize":
                SendResponse(request.Id, new { 
                protocolVersion = "2024-11-05", 
                capabilities = new { 
                tools = new { } // This tells the client: "I support tool calls"
        }, 
        serverInfo = new { name = "GademaServer", version = "1.0.0" } 
    });
                break;

            case "tools/list":
                var toolList = _tools.Values.Select(t => new 
                {
                    name = t.Name,
                    description = t.Description,
                    inputSchema = new { type = "object", properties = t.GetParameterDescriptions().ToDictionary(p => p.Key, p => new { type = "string", description = p.Value }) }
                });
                SendResponse(request.Id, new { tools = toolList });
                break;

            case "tools/call":
                await ExecuteTool(request);
                break;

            default:
                SendError(request.Id, $"Method '{request.Method}' not found.");
                break;
        }
    }

    private async Task ExecuteTool(McpRequest request)
    {
        var argsDict = new Dictionary<string, string>();
        if (request.Params.ValueKind != JsonValueKind.Undefined && request.Params.TryGetProperty("arguments", out var argsJson))
        {
            foreach (var prop in argsJson.EnumerateObject())
            {
                argsDict[prop.Name] = prop.Value.GetString() ?? "";
            }
        }

        string toolName = request.Params.GetProperty("name").GetString() ?? "";

        if (_tools.TryGetValue(toolName, out var tool))
        {
            try
            {
                string result = await tool.ExecuteAsync(argsDict);
                if(result.Contains("Error"))
                    Logger.LogError(result);
                SendResponse(request.Id, new { content = new[] { new { type = "text", text = result } } });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in Tool Execution: {ex.Message}", ex);
                SendError(request.Id, $"Tool Execution Error: {ex.Message}");
            }
        }
        else
        {
            SendError(request.Id, $"Tool '{toolName}' not found.");
        }
    }

    private void SendResponse(object id, object result) => 
        Console.WriteLine(JsonSerializer.Serialize(new { jsonrpc = "2.0", id, result }));

    private void SendError(object? id, string message) => 
        Console.WriteLine(JsonSerializer.Serialize(new { jsonrpc = "2.0", id, error = new { code = -32603, message } }));
}