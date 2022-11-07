using System;
using System.Reflection;
using System.Threading.Tasks;

namespace ServiceProviderEndpoint.Client;

internal static class TaskExtensions
{
    static readonly MethodInfo CastMethod = typeof(TaskExtensions).GetMethod(nameof(Cast), new[] { Types.Task })!;

    public static object? Cast(this Task task, Type targetType)
    {
        return CastMethod.MakeGenericMethod(targetType).Invoke(null, new[] { task });
    }

    public static async Task<T?> Cast<T>(this Task task)
    {
        await task.ConfigureAwait(false);
        var type = task.GetType();

        if (!type.IsGenericType)
            return default;

        return (T?)type.GetProperty(nameof(Task<object>.Result))!.GetValue(task);
    }
}
