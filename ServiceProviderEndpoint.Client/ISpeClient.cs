using System;
using System.Threading;

namespace ServiceProviderEndpoint.Client;

public interface ISpeClient : IServiceProvider
{
    object GetService(Type serviceType, CancellationToken cancellationToken);
}

public static class SpeClientExtensions
{
    public static T GetService<T>(this ISpeClient speClient, CancellationToken cancellationToken = default)
    {
        return (T)speClient.GetService(typeof(T), cancellationToken);
    }
}
