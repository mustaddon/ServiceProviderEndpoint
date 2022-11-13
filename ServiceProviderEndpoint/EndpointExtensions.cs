using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using ServiceProviderEndpoint;
using System.Reflection;

namespace Microsoft.AspNetCore.Builder;

public static class ServiceProviderEndpointExtensions
{
    public static IEndpointConventionBuilder MapServiceProvider(this IEndpointRouteBuilder builder, string route, IEnumerable<ServiceDescriptor> serviceDescriptors, IEnumerable<Type> extensions, Action<SpeOptions>? optionsConfigurator = null)
    {
        var pattern = $"{route.TrimEnd('/')}/{{service:required}}/{{member:required}}/{{parameters?}}";
        var options = new SpeOptions(); optionsConfigurator?.Invoke(options);
        var processor = new EndpointProcessor(serviceDescriptors, extensions, options);

        return new EndpointConventionBuilder(new[]
        {
            builder.MapGet(pattern, processor.ProcessGet),
            builder.MapPost(pattern, processor.ProcessPost),
        });
    }
    public static IEndpointConventionBuilder MapServiceProvider(this IEndpointRouteBuilder builder, string route, IEnumerable<ServiceDescriptor> serviceDescriptors, params Type[] extensions)
    {
        return builder.MapServiceProvider(route, serviceDescriptors, extensions.AsEnumerable(), null);
    }
}