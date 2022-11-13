namespace Test.Services;

public interface IGenericService<T>
{
    T? Prop { get; }
    T? Method(T a);
}
