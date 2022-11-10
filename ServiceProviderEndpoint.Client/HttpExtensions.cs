using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderEndpoint.Client;

internal static class HttpExtensions
{

    public static void EnsureSuccessStatusCodeDisposable(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        response.Dispose();
        throw new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase})");
    }


#if NETSTANDARD2_0
#pragma warning disable IDE0060 // Remove unused parameter
    public static Task<Stream> ReadAsStreamAsync(this HttpContent httpContent, CancellationToken cancellationToken) => httpContent.ReadAsStreamAsync();
    public static Task<string> ReadAsStringAsync(this HttpContent httpContent, CancellationToken cancellationToken) => httpContent.ReadAsStringAsync();
#pragma warning restore IDE0060 // Remove unused parameter
#endif

}
