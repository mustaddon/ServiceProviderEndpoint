﻿namespace Test.Services;

public static class GenericServiceExtensions
{
    public static T? MethodExt<T>(this IGenericService<T> svc, T? a) => svc.Method(a);
    public static int MethodExtInt(this IGenericService<int> svc, int a, int b) => svc.Method(a + b);
}
