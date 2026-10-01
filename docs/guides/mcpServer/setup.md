Since we have built a professional, modular, and extensible architecture, you need documentation that explains how to use it as both a **human** (via CLI) and an **agent** (via Continue).

Here is a comprehensive guide for your project. You can save this as `README_MCP.md` in your repository.

***

# 🚀 Gadema MCP Server Guide

This repository contains a custom **Model Context Protocol (MCP)** server implemented in C#. It provides high-performance, context-aware tools designed specifically for navigating and editing the Gadema codebase.

## 🧠 The Vision: "Project-Specific Intelligence"
Unlike generic AI tools, this server is built to live *inside* your project. It uses **Reflection** to automatically discover new tools added to the `Tools/` directory and provides a bridge between the LLM's reasoning and your local file system.

---

## 🛠️ Setup & Installation

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [VS Code](https://code.visualstudio.com/) with the **Continue** extension.

### 1. Build the Project
Ensure all dependencies are restored and the project is built:
```bash
dotnet build Gadema.McpServer/Gadema.McpServer.csproj
```

### 2. Configure Continue (The "Hands")
To allow the AI to use these tools, add the following configuration to your `config.yaml` in the Continue extension settings:

```yaml
mcpServers:
  - name: gadema-server
    command: dotnet
    args:
      - run
      - --project
      - /absolute/path/to/your/project/Gadema.McpServer/Gadema.McpServer.csproj
```
> **Note:** Replace `/absolute/path/to/your/project/` with the actual path on your machine.

---

## 🤖 Using the Tools in AI Chat (Agent Mode)

Once configured, you don't need to run commands manually. Simply talk to the AI. It will use the `gadema-server` tools automatically.

### 🔍 Capability: Reading Changes
**Tool:** `diff_files`  
**Use Case:** Ask the AI to analyze what changed between two versions of a file.
> *"Compare the changes between `src/Models/User.cs` and `src/Models/User_old.cs`."*

### ✍️ Capability: Safe Editing (Smart Refactoring)
**Tool:** `apply_hunk`  
**Use Case:** Ask the AI to apply a change. The tool uses **Context-Based Matching** to ensure it only applies the edit if the surrounding code matches perfectly, preventing "Line Drift" errors.
> *"Apply this hunk to `src/Models/User.cs`:*
> ```text
> {
> -    public int id;
+    public Guid id;
> }
> ```"*

### 🧪 Capability: Connectivity Test
**Tool:** `health_check`  
**Use Case:** Verify the connection between Continue and your C# server.
> *"Run a health check on the gadema-server."*

---

## 💻 Manual Testing (Human Mode)

You can test any tool manually via your terminal without using an AI. This is highly recommended for debugging your logic.

### 1. View the Auto-Generated Help Menu
The server automatically generates documentation based on your code's `[Description]` and `[Option]` attributes:
```bash
dotnet run --project Gadema.McpServer/Gadema.McpServer.csproj -- --cli help
```

### 2. Execute a Tool Manually
Use the `--cli` flag followed by the command and its arguments:

**Test Diffing:**
```bash
dotnet run --project Gadema.McpServer/Gadema.McpServer.csproj -- --cli diff_files --file1=fileA.cs --file2=fileB.cs
```

**Test Applying a Hunk:**
```bash
dotnet run --project Gadema.McpServer/Gadema.McpServer.csproj -- --cli apply_hunk --file_path=test.cs --hunk="context line\n-old line\n+new line"
```

---

## 🏗️ Developer Guide: Adding New Tools

This system is designed for **Zero-Maintenance Expansion**. To add a new tool:

1.  Create a new class in `Gadema.McpServer/Tools/`.
2.  Implement the `IMcpTool` interface.
3.  Define your parameters and description.
4.  **That's it!** The server will automatically detect, register, and document your tool on the next run.

```csharp
public class MyNewTool : IMcpTool
{
    public string Name => "my_tool";
    public string Description => "What this tool does.";
    public Dictionary<string, string> GetParameterDescriptions() => new() { { "arg1", "desc" } };

    public async Task<string> ExecuteAsync(Dictionary<string, string> args)
    {
        // Your logic here
        return "Success";
    }
}
```