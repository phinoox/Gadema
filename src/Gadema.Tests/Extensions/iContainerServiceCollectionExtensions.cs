using Microsoft.Extensions.DependencyInjection;

namespace Gadema.Tests.Extensions;

public static class ScopedServiceProviderExtensions
{
    /// <summary>
    /// Creates a scoped service provider from the web host.
    /// </summary>
    public static IServiceScope CreateScoped(this IServiceProvider serviceProvider) =>
        serviceProvider.CreateScope();
}