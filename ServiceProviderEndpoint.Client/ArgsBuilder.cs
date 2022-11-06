using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ServiceProviderEndpoint.Client
{
    internal class ArgsBuilder
    {
        public static List<object?> Build(MethodInfo method, object?[] args, out List<object> streamables, out List<CancellationToken> cTockens)
        {
            var result = new List<object?>();
            streamables = new List<object>();
            cTockens = new List<CancellationToken>();

            var parameters = method.GetParameters();
            var count = Math.Min(args.Length, parameters.Length);

            for (var i = 0; i < count; i++)
            {
                var arg = args[i];
                var parameterType = parameters[i].ParameterType;

                if (parameterType.IsAssignableFrom(Types.CancellationToken))
                {
                    if (arg is CancellationToken cancellationToken) 
                        cTockens.Add(cancellationToken);
                }
                else if (parameterType.IsStreamable())
                    streamables.Add(arg ?? Stream.Null);
                else
                    result.Add(arg);
            }

            return result;
        }
    }
}
