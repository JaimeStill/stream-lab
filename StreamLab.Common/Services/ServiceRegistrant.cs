using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace StreamLab.Common.Services;
public abstract class ServiceRegistrant
{
    protected readonly IServiceCollection services;

    public ServiceRegistrant(IServiceCollection services)
    {
        this.services = services;
    }

    public abstract void Register();
}

public static class ServiceRegistration
{
    public static void AddAppServices(this IServiceCollection services)
    {
        Assembly? entry = Assembly.GetEntryAssembly();

        if (entry is not null)
        {
            IEnumerable<Assembly> assemblies = entry
                .GetReferencedAssemblies()
                .Select(Assembly.Load)
                .Append(entry)
                .Where(x =>
                    x.GetTypes()
                        .Any(y =>
                            y.IsClass
                            && !y.IsAbstract
                            && y.IsSubclassOf(typeof(ServiceRegistrant))
                        )
                );

            IEnumerable<Type>? registrants = assemblies
                .SelectMany(x =>
                    x.GetTypes()
                        .Where(x =>
                            x.IsClass
                            && !x.IsAbstract
                            && x.IsSubclassOf(typeof(ServiceRegistrant))
                        )
                );

            if (registrants is not null)
                foreach (Type registrant in registrants)
                    ((ServiceRegistrant?)Activator.CreateInstance(registrant, services))?.Register();
        }
    }
}