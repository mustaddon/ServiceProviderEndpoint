using DispatchProxyAdvanced;
using MetaFile;
using MetaFile.Http;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TypeSerialization.Json;

namespace ServiceProviderEndpoint.Client;

public class SpeClient : ISpeClient, IDisposable
{
    public SpeClient(string address, Action<SpeClientSettings>? settingsConfigurator = null)
    {
        _client = new(() => CreateClient(address));
        _settings = new SpeClientSettings(); settingsConfigurator?.Invoke(_settings);
        _settings.JsonSerializerOptions.Converters.Add(new JsonTypeConverter(_settings.TypeDeserializer));
    }

    private readonly SpeClientSettings _settings;
    private readonly Lazy<HttpClient> _client;


    public HttpRequestHeaders DefaultRequestHeaders => _client.Value.DefaultRequestHeaders;

    public void Dispose()
    {
        if (_client.IsValueCreated)
            _client.Value.Dispose();

        GC.SuppressFinalize(this);
    }

    public ISpeServiceRequest<TService> CreateRequest<TService>() => new SpeServiceRequest<TService>(this);

    public object GetService(Type serviceType) => GetService(serviceType, CancellationToken.None);

    public object GetService(Type serviceType, CancellationToken cancellationToken)
    {
        if (!serviceType.IsInterface)
            throw new ArgumentException("For service types that are not interfaces, use 'GetServiceUnsafe' method.");

        return GetServiceUnsafe(serviceType, cancellationToken);
    }

    public object GetServiceUnsafe(Type serviceType, CancellationToken cancellationToken)
    {
        return ProxyFactory.CreateInstance(serviceType, (p, method, args) =>
        {
            var returnAsTask = Types.Task.IsAssignableFrom(method.ReturnType);

            var resultType = !returnAsTask ? method.ReturnType
                : method.ReturnType.IsGenericType ? method.ReturnType.GetGenericArguments().First()
                : Types.Void;

            var requestTask = CreateRequest(serviceType, method, null, args, cancellationToken);
            var resultTask = GetResult(requestTask, resultType, cancellationToken);

            if (!returnAsTask)
                return resultTask.ConfigureAwait(false).GetAwaiter().GetResult();

            if (!method.ReturnType.IsGenericType)
                return resultTask;

            return resultTask.Cast(resultType);
        });
    }

    internal virtual Task<HttpResponseMessage> CreateRequest(Type service, MemberInfo member, Type?[]? parameters, object?[] args, CancellationToken cancellationToken)
    {
        var requestArgs = ArgsBuilder.Build(member, args, out var streamables, out var cancellationTokens);
        var streamable = streamables.FirstOrDefault();

        if (cancellationTokens.Any())
            cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(new[] { cancellationToken }.Concat(cancellationTokens).Distinct().ToArray()).Token;

        if (streamable == null)
            return _client.Value.PostAsJsonAsync(UriBuilder.Build(service, member, parameters, args), requestArgs, _settings.JsonSerializerOptions, cancellationToken);

        var queryArgs = JsonSerializer.Serialize(requestArgs, _settings.JsonSerializerOptions);
        var requestUri = UriBuilder.Build(service, member, parameters, args, queryArgs);

        if (streamable is IStreamFileReadOnly file)
            return _client.Value.PostAsStreamFile(requestUri, file, _settings.JsonSerializerOptions, cancellationToken);

        if (streamable is Stream stream)
            return _client.Value.PostAsync(requestUri, new StreamContent(stream), cancellationToken);

        throw new InvalidDataException($"Unsupported streamable type {streamable.GetType()}");
    }

    internal virtual async Task<object?> GetResult(Task<HttpResponseMessage> requestTask, Type resultType, CancellationToken cancellationToken)
    {
        var response = await requestTask;

        response.EnsureSuccessStatusCodeDisposable();

        if (response.StatusCode == HttpStatusCode.NoContent || resultType.Equals(Types.Void))
            return null;

        if (resultType.Equals(Types.Object) && response.Headers.TryGetValues(Headers.ResultType, out var resultTypeHeaders))
            resultType = _settings.TypeDeserializer.Deserialize(resultTypeHeaders.First())!;

        if (resultType.Equals(Types.Stream))
            return new SpeStream(await response.Content.ReadAsStreamAsync(cancellationToken), () => response.Dispose());

        if (!resultType.Equals(Types.Object))
        {
            if (resultType.IsAssignableFrom(Types.StreamFile))
                return await response.ReadAsStreamFile(Types.StreamFile, _settings.JsonSerializerOptions, cancellationToken);

            if (!resultType.IsAbstract && Types.IStreamFile.IsAssignableFrom(resultType))
                return await response.ReadAsStreamFile(resultType, _settings.JsonSerializerOptions, cancellationToken);
        }

        using (response)
        {
            return await response.Content.ReadFromJsonAsync(resultType, _settings.JsonSerializerOptions, cancellationToken);
        }
    }


    protected virtual HttpClient CreateClient(string address)
    {
        if (!address.EndsWith("/"))
            address += "/";

        var handler = _settings.Credentials != null
            ? new HttpClientHandler { Credentials = _settings.Credentials }
            : new HttpClientHandler { UseDefaultCredentials = true };

        var client = new HttpClient(handler) { BaseAddress = new Uri(address) };

        foreach (var kvp in _settings.DefaultRequestHeaders)
            client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);

        return client;
    }
}
