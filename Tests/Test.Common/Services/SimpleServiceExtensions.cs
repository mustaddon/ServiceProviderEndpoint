namespace Test.Services;

public static class SimpleServiceExtensions
{

    public static int MethodValExt(this ISimpleService svc, int a, int b, int c) => svc.MethodVal(a, b + c);
    public static string? MethodRefExt<T>(this ISimpleService svc, string a, T b) => svc.MethodRef(a + b?.ToString());

}
