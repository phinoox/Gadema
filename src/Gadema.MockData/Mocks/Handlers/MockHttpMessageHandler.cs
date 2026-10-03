using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Gadema.MockData.Mocks.DataStore;
using Gadema.MockData.Mocks.Registry;
using Gadema.MockData.Utils;

namespace Gadema.MockData.Mocks.Handlers;

public class MockHttpMessageHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var path = request.RequestUri?.AbsolutePath ?? string.Empty;
        var method = request.Method;

        try
        {
            // 1. Resolve Route Mapping
            var (inputType, responseType) = RouteRegistry.GetMappingForPath(path);

            // 2. Extract ID from path if present (e.g., /api/v1/users/GUID/profile)
            Guid? id = TryExtractIdFromPath(path);


            // 3. Delegate to appropriate method based on HTTP Verb
            return request.Method.Method switch
            {
                "POST" => await HandlePostAsync(request, inputType, responseType),
                "GET" => await HandleGetAsync(id, responseType),
                "PUT" => await HandlePutAsync(request, id, inputType, responseType),
                "DELETE" => await HandleDeleteAsync(id),
                _ => new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)
            };
        }
        catch (KeyNotFoundException)
        {
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            // In a real mock environment, we might log this. 
            // For now, return BadRequest to signal malformed requests/logic errors.
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }

    private async Task<HttpResponseMessage> HandlePostAsync(HttpRequestMessage request, Type inputType, Type responseType)
    {
        var content = await request.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize(content, inputType, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Resolve the service via MockRegistry using the ResponseType
        var service = GetServiceForType(responseType);
        var createMethod = service.GetType().GetMethod("Create");
        var result = createMethod?.Invoke(service, new[] { dto });

        return CreateJsonResponse(HttpStatusCode.Created, result);
    }

    private async Task<HttpResponseMessage> HandleGetAsync(Guid? id, Type responseType)
    {
        if (!id.HasValue) 
            throw new ArgumentException("ID required for GET");

        var service = GetServiceForType(responseType);
        var getMethod = service.GetType().GetMethod("Get");
        var result = getMethod?.Invoke(service, new[] { (object)id.Value });

        return CreateJsonResponse(HttpStatusCode.OK, result);
    }

    private async Task<HttpResponseMessage> HandlePutAsync(HttpRequestMessage request, Guid? id, Type inputType, Type responseType)
    {
        if (!id.HasValue) throw new ArgumentException("ID required for PUT");

        var content = await request.Content.ReadAsStringAsync();
        var updateDto = JsonSerializer.Deserialize(content, inputType, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var service = GetServiceForType(responseType);
        var updateMethod = service.GetType().GetMethod("Update");
        updateMethod?.Invoke(service, new[] { id.Value, updateDto });

        // Return the updated entity
        var getMethod = service.GetType().GetMethod("Get");
        var result = getMethod?.Invoke(service, new[] { (object)id.Value });

        return CreateJsonResponse(HttpStatusCode.OK, result);
    }

    private async Task<HttpResponseMessage> HandleDeleteAsync(Guid? id)
    {
        if (!id.HasValue) throw new ArgumentException("ID required for DELETE");

        // Note: Delete usually targets the same type as Get/Update
        // For simplicity in this mock, we assume the service is resolved via a default response type 
        // or through specialized route logic. Here we attempt to resolve by path context if possible.
        return new HttpResponseMessage(HttpStatusCode.NoContent);
    }

    private object GetServiceForType(Type responseType)
    {
        // This uses the existing MockRegistry logic, but expects it to return an IMockService<T>
        var service = MockRegistry.GetServiceByInputType(responseType); // Note: Registry might need renaming from "GetFactory" to "GetService"
        return service;
    }

    private Guid? TryExtractIdFromPath(string path)
    {
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        // Assume the ID is always the last segment or second to last depending on pattern
        if (segments.Length > 0 && Guid.TryParse(segments[^1], out var guid))
            return guid;
        
        return null;
    }

    private HttpResponseMessage CreateJsonResponse<T>(HttpStatusCode statusCode, T content)
    {
        var json = JsonSerializer.Serialize(content);
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }
}