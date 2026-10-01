using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gadema.McpServer.Tools;

public class HealthCheckTool : IMcpTool
{
    public string Name => "health_check";
    public string Description => "A simple tool to verify the MCP server is alive and responding correctly.";

    // This tool requires no parameters
    public Dictionary<string, string> GetParameterDescriptions() => new();

    public async Task<string> ExecuteAsync(Dictionary<string, string> args)
    {
        // We return a structured JSON response to prove the pipe is clean
        return "{\"status\": \"ok\", \"message\": \"Server is healthy and responding to MCP calls.\"}";
    }
}