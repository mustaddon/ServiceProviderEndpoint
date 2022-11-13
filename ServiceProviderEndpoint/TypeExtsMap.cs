using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ServiceProviderEndpoint;

internal static class TypeExtsMap
{

    public static Dictionary<Type, HashSet<MethodInfo>> Create(IEnumerable<ServiceDescriptor> ServiceDescriptors, IEnumerable<Type> extensions)
    {
        var map = new Dictionary<Type, HashSet<MethodInfo>>();

        var services = new HashSet<Type>(ServiceDescriptors.Select(x => x.ServiceType));

        var extGroups = extensions.Where(x => x.IsStatic())
            .SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .Where(x => x.IsExtension())
            .GroupBy(x =>
            {
                var param = x.GetParameters().First().ParameterType;
                return param.IsGenericType && param.GenericTypeArguments.Any(x => x.IsGenericParameter)
                    ? param.GetGenericTypeDefinition()
                    : param;
            });

        foreach (var extGroup in extGroups)
            foreach (var service in services)
            {
                if (extGroup.Key.IsAssignableFrom(service))
                {
                    if (!map.TryGetValue(service, out var serviceExts))
                        map.Add(service, (serviceExts = new()));

                    foreach (var ext in extGroup)
                        serviceExts.Add(ext);

                    continue;
                }

                if (service.IsGenericType && service.IsConstructedGenericType)
                {
                    var genericOpen = service.GetGenericTypeDefinition();

                    if (extGroup.Key.IsAssignableFrom(genericOpen))
                    {
                        if (!map.TryGetValue(genericOpen, out var genericOpenExts))
                            map.Add(genericOpen, (genericOpenExts = new()));

                        foreach (var ext in extGroup)
                            genericOpenExts.Add(ext);

                        continue;
                    }
                }
            }

        return map;
    }

}
