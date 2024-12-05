
using System.Reflection;


namespace Api.Config
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly(); // Ensamblado actual

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract)
                {
                    var interfaces = type.GetInterfaces();
                    foreach (var iface in interfaces)
                    {
                        if (iface.Name.EndsWith("Repository")) // Convención: las interfaces terminan en "Repository"
                        {
                            services.AddScoped(iface, type);
                        }
                    }
                }
            }

            return services; // Retorna el IServiceCollection para que puedas encadenar más servicios si lo deseas
        }
    }
}
