// src/Gadema.Api/Services/Core/ServiceLifetimeAttribute.cs
namespace Gadema.Api.Services.Core;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceLifetimeAttribute : Attribute
{
    public ServiceLifetime Lifetime { get; }
    public ServiceLifetimeAttribute(ServiceLifetime lifetime) => Lifetime = lifetime;
}