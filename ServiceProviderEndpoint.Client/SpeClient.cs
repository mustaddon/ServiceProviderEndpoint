using DispatchProxyAdvanced;
using SingleApi;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderEndpoint.Client;

public class SpeClient : ISpeClient, IServiceProvider, IDisposable
{
    public SpeClient(string address, SpeClientSettings? settings = null)
    {
        _settings = settings ?? SpeClientSettings.Default;
        _client = new(() => CreateClient(address));
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

    public object GetService(Type serviceType) => GetService(serviceType, CancellationToken.None);

    public object GetService(Type serviceType, CancellationToken cancellationToken)
    {
        return ProxyFactory.Create(serviceType, (method, args) =>
        {
            var returnAsTask = Types.Task.IsAssignableFrom(method.ReturnType);
            
            var resultType = !returnAsTask ? method.ReturnType
                : method.ReturnType.IsGenericType ? method.ReturnType.GetGenericArguments().First()
                : Types.Void;

            var requestTask = CreateRequest(serviceType, method, args, cancellationToken);
            var resultTask = GetResult(requestTask, resultType, cancellationToken);
            
            if (!returnAsTask)
                return resultTask.ConfigureAwait(false).GetAwaiter().GetResult();

            if (!method.ReturnType.IsGenericType)
                return resultTask;

            return resultTask.Cast(resultType);
        });
    }

    protected virtual Task<HttpResponseMessage> CreateRequest(Type service, MethodInfo method, object?[] args, CancellationToken cancellationToken)
    {
        var requestArgs = ArgsBuilder.Build(method, args, out var streamables, out var cancellationTokens);
        var streamable = streamables.FirstOrDefault();

        if (cancellationTokens.Any())
            cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(new[] { cancellationToken }.Concat(cancellationTokens).Distinct().ToArray()).Token;

        if (streamable == null)
            return _client.Value.PostAsJsonAsync(UriBuilder.Build(service, method, args), requestArgs, _settings.JsonSerializerOptions, cancellationToken);

        var queryArgs = JsonSerializer.Serialize(requestArgs, _settings.JsonSerializerOptions);
        var requestUri = UriBuilder.Build(service, method, args, queryArgs);

        if (streamable is ISapiFileReadOnly file)
            return _client.Value.PostAsSapiFile(requestUri, file, _settings.JsonSerializerOptions, cancellationToken);

        if (streamable is Stream stream)
            return _client.Value.PostAsync(requestUri, new StreamContent(stream), cancellationToken);

        throw new InvalidDataException($"Unsupported streamable type {streamable.GetType()}");
    }

    protected virtual async Task<object?> GetResult(Task<HttpResponseMessage> requestTask, Type resultType, CancellationToken cancellationToken)
    {
        var response = await requestTask;

        response.EnsureSuccessStatusCodeDisposable();

        if (resultType.Equals(Types.Void))
            return null;

        if (resultType.Equals(Types.Stream))
            return new SpeStreamWrapper(await response.Content.ReadAsStreamAsync(cancellationToken), () => response.Dispose());

        if (resultType.IsAssignableFrom(Types.SapiFile))
            return await response.ToSapiFile(Types.SapiFile, _settings.JsonSerializerOptions, cancellationToken);

        if (!resultType.IsAbstract && Types.ISapiFile.IsAssignableFrom(resultType))
            return await response.ToSapiFile(resultType, _settings.JsonSerializerOptions, cancellationToken);

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
