namespace Example.WebApi.Services;

public class ExampleGenericService<T> : IExampleGenericService<T>
{
    public Task<T> SimpleMethod(T a, CancellationToken cancellationToken = default) => Task.FromResult(a);
}
