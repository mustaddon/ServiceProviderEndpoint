using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using ServiceProviderEndpoint;

namespace Microsoft.AspNetCore.Builder;

public static class ServiceProviderEndpointExtensions
{
    /// <summary>
    /// Adds IServiceProvider as <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that handles HTTP requests.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="route">The route pattern.</param>
    /// <param name="serviceTypes">Service types from IServiceCollection.</param>
    /// <param name="extensions">Object types for extensions, casting or resolving.</param>
    /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
    public static IEndpointConventionBuilder MapServiceProvider(this IEndpointRouteBuilder builder, string route, IEnumerable<Type> serviceTypes, IEnumerable<Type>? extensions = null, Action<SpeOptions>? optionsConfigurator = null)
    {
        var pattern = $"{route.TrimEnd('/')}/{{service:required}}/{{member:required}}/{{parameters?}}";
        var options = new SpeOptions(); optionsConfigurator?.Invoke(options);
        var processor = new EndpointProcessor(serviceTypes, extensions ?? Array.Empty<Type>(), options);

        return new EndpointConventionBuilder(new[]
        {
            builder.MapGet(pattern, processor.ProcessGet),
            builder.MapPost(pattern, processor.ProcessPost),
        });
    }

    /// <summary>
    /// Adds IServiceProvider as <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that handles HTTP requests.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="route">The route pattern.</param>
    /// <param name="serviceDescriptors">Service descriptors from IServiceCollection.</param>
    /// <param name="extensions">Object types for extensions, casting or resolving.</param>
    /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
    public static IEndpointConventionBuilder MapServiceProvider(this IEndpointRouteBuilder builder, string route, IEnumerable<ServiceDescriptor> serviceDescriptors, IEnumerable<Type> extensions, Action<SpeOptions>? optionsConfigurator = null)
    {
        return builder.MapServiceProvider(route, serviceDescriptors.Select(x => x.ServiceType), extensions, optionsConfigurator);
    }


    /// <summary>
    /// Adds IServiceProvider as <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that handles HTTP requests.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="route">The route pattern.</param>
    /// <param name="serviceDescriptors">Service descriptors from IServiceCollection.</param>
    /// <param name="extensions">Object types for extensions, casting or resolving.</param>
    /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
    public static IEndpointConventionBuilder MapServiceProvider(this IEndpointRouteBuilder builder, string route, IEnumerable<ServiceDescriptor> serviceDescriptors, params Type[] extensions)
    {
        return builder.MapServiceProvider(route, serviceDescriptors, extensions.AsEnumerable(), null);
    }

}