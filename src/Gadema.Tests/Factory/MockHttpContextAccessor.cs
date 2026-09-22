using System.Security.Principal;
using Microsoft.AspNetCore.Http;

namespace Gadema.Tests.Factory;

public class MockHttpContextAccessor : IHttpContextAccessor
{
    public HttpContext HttpContext { get; set; } = new DefaultHttpContext
    {
        User = new GenericPrincipal(
            new GenericIdentity("test-user"), 
            new[] { "Role1", "Role2" })
        
    };
}