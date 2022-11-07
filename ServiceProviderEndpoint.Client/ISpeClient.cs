using System;
using System.Linq.Expressions;
using System.Threading;

namespace ServiceProviderEndpoint.Client;

public interface ISpeClient : IServiceProvider
{
    ISpeRequestBuilder<TService> CreateRequest<TService>();
    object GetService(Type serviceType, CancellationToken cancellationToken);
    object GetServiceUnsafe(Type serviceType, CancellationToken cancellationToken = default);
}

public static class SpeClientExtensions
{
    public static T GetService<T>(this ISpeClient speClient, CancellationToken cancellationToken = default)
    {
        return (T)speClient.GetService(typeof(T), cancellationToken);
    }

    public static T GetServiceUnsafe<T>(this ISpeClient speClient, CancellationToken cancellationToken = default)
    {
        return (T)speClient.GetServiceUnsafe(typeof(T), cancellationToken);
    }
}
