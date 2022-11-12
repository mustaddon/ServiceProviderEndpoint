namespace Test.Client;

public partial class Test : IDisposable
{
    readonly Random _rnd = new Random();

    readonly SpeClient _client = new($"{Settings.WebApiUrl}services", x => x.DefaultRequestHeaders = new() {
        { "spe-test", new [] { "test_value" } },
    });

    public void Dispose()
    {
        _client?.Dispose();
        GC.SuppressFinalize(this);
    }

}
