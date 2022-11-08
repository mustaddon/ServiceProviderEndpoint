using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using ServiceProviderEndpoint;
using System.Reflection;

namespace Microsoft.AspNetCore.Builder;

public static class ServiceProviderEndpointExtensions
{
    public static IEndpointConventionBuilder MapServiceProvider(this IEndpointRouteBuilder builder, string route, IEnumerable<ServiceDescriptor> serviceDescriptors, IEnumerable<Type> additionalTypes, Action<SpeOptions>? optionsConfigurator = null)
    {
        var pattern = $"{route.TrimEnd('/')}/{{service:required}}/{{member:required}}/{{parameters?}}";
        var options = new SpeOptions(); optionsConfigurator?.Invoke(options);
        var processor = new EndpointProcessor(serviceDescriptors, additionalTypes, options);

        return new EndpointConventionBuilder(new[]
        {
            builder.MapGet(pattern, processor.ProcessGet),
            builder.MapPost(pattern, processor.ProcessPost),
        });
    }

    public static IEndpointConventionBuilder MapServiceProvider(this IEndpointRouteBuilder builder, string route, IEnumerable<ServiceDescriptor> serviceDescriptors, IEnumerable<Assembly> additionalAssemblies, Action<SpeOptions>? optionsConfigurator = null)
    {
        return builder.MapServiceProvider(route, serviceDescriptors,
            additionalAssemblies.SelectMany(x => x.GetTypes())
                .Where(x => x.IsPublic && !x.IsStatic() && !Types.Attribute.IsAssignableFrom(x)),
            optionsConfigurator);
    }

    public static IEndpointConventionBuilder MapServiceProvider(this IEndpointRouteBuilder builder, string route, IEnumerable<ServiceDescriptor> serviceDescriptors, params Assembly[] additionalAssemblies)
    {
        return builder.MapServiceProvider(route, serviceDescriptors, additionalAssemblies.AsEnumerable(), null);
    }
}