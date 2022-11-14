namespace Test.Client;

public partial class Test : IDisposable
{
    readonly Random _rnd = new();

    readonly SpeClient _client = new($"{Settings.WebApiUrl}services");

    public void Dispose()
    {
        _client?.Dispose();
        GC.SuppressFinalize(this);
    }

}
