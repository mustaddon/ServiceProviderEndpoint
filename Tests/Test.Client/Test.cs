

namespace Test.Client;

public partial class Test : IDisposable
{
    readonly Random _rnd = new Random();

    readonly SpeClient _client = new($"{Settings.WebApiUrl}services", new()
    {
        DefaultRequestHeaders = new() {
            { "sapi-test", new [] { "test_value" } },
        }
    });

    public void Dispose()
    {
        _client?.Dispose();
        GC.SuppressFinalize(this);
    }

}
