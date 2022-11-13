namespace Example;

public static class ExampleServiceExtensions
{
    public static int SimpleMethodExt(this IExampleService svc, int a, int b, int c) => svc.SimpleMethod(a, b + c);
}
