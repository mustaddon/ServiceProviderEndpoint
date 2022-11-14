using System;
using System.Threading;

namespace ServiceProviderEndpoint.Client;

public interface ISpeClient : IServiceProvider
{
    /// <summary>
    /// Create a service request builder.
    /// </summary>
    /// <typeparam name="TService">The type of service object.</typeparam>
    /// <returns>A <see cref="ISpeServiceRequest{TService}"/> that can be used to further build the request.</returns>
    ISpeServiceRequest<TService> CreateRequest<TService>();

    /// <summary>
    /// Get proxy service of interface type from the <see cref="ISpeClient"/>.
    /// </summary>
    /// <remarks>For any types use method <see cref="GetServiceUnsafe(Type, CancellationToken)"/></remarks>
    /// <param name="serviceType">The type of service object to get.</param>
    /// <returns>A proxy service object of type.</returns>
    /// <exception cref="System.ArgumentException">Type is not an interface.</exception>
    object GetService(Type serviceType, CancellationToken cancellationToken);

    /// <summary>
    /// Get proxy service of type from the <see cref="ISpeClient"/>.
    /// </summary>
    /// <remarks>WARNING: Only virtual members of type will be handled correctly.</remarks>
    /// <param name="serviceType">The type of service object to get.</param>
    /// <returns>A proxy service object of type.</returns>
    /// <exception cref="System.ArgumentException">Type is sealed.</exception>
    object GetServiceUnsafe(Type serviceType, CancellationToken cancellationToken = default);
}

public static class SpeClientExtensions
{
    /// <summary>
    /// Get proxy service of interface <typeparamref name="T"/> from the <see cref="ISpeClient"/>.
    /// </summary>
    /// <remarks>For any types use method <see cref="GetServiceUnsafe"/></remarks>
    /// <typeparam name="T">The interface of service object to get.</typeparam>
    /// <param name="speClient">The <see cref="ISpeClient"/> to retrieve the service object from.</param>
    /// <returns>A proxy service object of type <typeparamref name="T"/>.</returns>
    /// <exception cref="System.ArgumentException"><typeparamref name="T"/> is not an interface.</exception>
    public static T GetService<T>(this ISpeClient speClient, CancellationToken cancellationToken = default)
    {
        return (T)speClient.GetService(typeof(T), cancellationToken);
    }

    /// <summary>
    /// Get proxy service of type <typeparamref name="T"/> from the <see cref="ISpeClient"/>.
    /// </summary>
    /// <remarks>WARNING: Only virtual members of type will be handled correctly.</remarks>
    /// <typeparam name="T">The type of service object to get.</typeparam>
    /// <param name="speClient">The <see cref="ISpeClient"/> to retrieve the service object from.</param>
    /// <returns>A proxy service object of type <typeparamref name="T"/>.</returns>
    /// <exception cref="System.ArgumentException"><typeparamref name="T"/> is sealed.</exception>
    public static T GetServiceUnsafe<T>(this ISpeClient speClient, CancellationToken cancellationToken = default)
    {
        return (T)speClient.GetServiceUnsafe(typeof(T), cancellationToken);
    }
}
