using System.Globalization;

namespace Test.Services;

public interface IGenericService<T>
{
    T? Prop { get; set; }
    T? Method(T? a);
    TB? MethodB<TA, TB>(TA? a, TB? b);
}
