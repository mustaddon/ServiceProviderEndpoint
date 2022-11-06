using SingleApi;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProviderEndpoint.Client
{
    internal static class Types
    {
        public static readonly Type Void = typeof(void);
        public static readonly Type Object = typeof(object);
        public static readonly Type Task = typeof(Task);
        public static readonly Type Stream = typeof(Stream);
        public static readonly Type SapiFile = typeof(SapiFile);
        public static readonly Type ISapiFile = typeof(ISapiFile);
        public static readonly Type ISapiFileReadOnly = typeof(ISapiFileReadOnly);
        public static readonly Type CancellationToken = typeof(CancellationToken);

        public static bool IsStreamable(this Type type) => Stream.IsAssignableFrom(type) || ISapiFileReadOnly.IsAssignableFrom(type);
    }
}
