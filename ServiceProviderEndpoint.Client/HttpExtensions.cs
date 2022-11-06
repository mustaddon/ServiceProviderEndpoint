﻿using SingleApi;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderEndpoint.Client;

internal static class HttpExtensions
{
    public static async Task<ISapiFile?> ToSapiFile(this HttpResponseMessage response, Type type, JsonSerializerOptions jsonOptions, CancellationToken cancellationToken)
    {
        var file = (ISapiFile)(Activator.CreateInstance(type) ?? throw new Exception($"Can not create instance of '{type}'."));

        file.Content = new SpeStreamWrapper(await response.Content.ReadAsStreamAsync(cancellationToken), () => response.Dispose());
        file.Name = response.Content.Headers.ContentDisposition?.FileNameStar ?? response.Content.Headers.ContentDisposition?.FileName;
        file.Type = response.Content.Headers.ContentType?.MediaType;

        if (file is ISapiFileResponse fileResponse)
            fileResponse.InlineDisposition = string.Equals(DispositionTypeNames.Inline, response.Content.Headers.ContentDisposition?.DispositionType, StringComparison.InvariantCultureIgnoreCase);

        if (response.Headers.TryGetValues(SapiHeaders.Metadata, out var metadataValues) && metadataValues.Any())
        {
            var metadataType = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISapiFile<>))?.GenericTypeArguments.First();

            if (metadataType != null)
            {
                var metadata = JsonSerializer.Deserialize(Uri.UnescapeDataString(metadataValues.First()), metadataType, jsonOptions);
                var metadataProp = type.GetProperty(nameof(ISapiFile<int>.Metadata));
                metadataProp!.SetValue(file, metadata);
            }
        }

        return file;
    }

    public static Task<HttpResponseMessage> PostAsStream(this HttpClient client, string uri, object? request, CancellationToken cancellationToken)
    {
        if (request is not Stream stream)
            throw new ArgumentNullException(nameof(request));

        var content = new StreamContent(stream);

        return client.PostAsync(uri, content, cancellationToken);
    }

    public static Task<HttpResponseMessage> PostAsSapiFile(this HttpClient client, string uri, ISapiFileReadOnly file, JsonSerializerOptions jsonOptions, CancellationToken cancellationToken)
    {
        var content = new StreamContent(file.Content);

        if (file.Type != null)
            content.Headers.ContentType = new MediaTypeHeaderValue(file.Type);

        content.Headers.ContentDisposition = new ContentDispositionHeaderValue(DispositionTypeNames.Attachment)
        {
            FileName = file.Name,
            FileNameStar = file.Name,
        };

        if (file is ISapiFileReadOnly<object> filePlus && filePlus.Metadata != null)
            content.Headers.TryAddWithoutValidation(SapiHeaders.Metadata, Uri.EscapeDataString(JsonSerializer.Serialize(filePlus.Metadata, jsonOptions)));

        return client.PostAsync(uri, content, cancellationToken);
    }

    public static void EnsureSuccessStatusCodeDisposable(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        response.Dispose();
        throw new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase})");
    }

    public static bool IsPlainText(this MediaTypeHeaderValue? value)
    {
        return string.Equals(value?.MediaType, "text/plain", StringComparison.InvariantCultureIgnoreCase);
    }


#if NETSTANDARD2_0
#pragma warning disable IDE0060 // Remove unused parameter
    public static Task<Stream> ReadAsStreamAsync(this HttpContent httpContent, CancellationToken cancellationToken) => httpContent.ReadAsStreamAsync();
    public static Task<string> ReadAsStringAsync(this HttpContent httpContent, CancellationToken cancellationToken) => httpContent.ReadAsStringAsync();
#pragma warning restore IDE0060 // Remove unused parameter
#endif

}