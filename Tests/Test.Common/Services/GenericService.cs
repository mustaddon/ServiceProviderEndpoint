namespace Test.Services;

public class GenericService<T> : IGenericService<T>
{
    public T? Prop { get; set; }
    public T? Method(T? a) => a;
    public TB? MethodB<TA, TB>(TA? a, TB? b) => b;
}
