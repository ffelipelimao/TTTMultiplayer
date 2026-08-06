using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

public static class ServiceCollectionsExtensions
{
    public static IServiceCollection AddPacketHandlers(this IServiceCollection services)
    {
        // Assembly == a compiled .dll, the rough equivalent of a .jar.
        // GetAssemblies() gives the ones already loaded into this process, so
        // this is a classpath scan: the same thing Spring's @ComponentScan or
        // the Reflections library does in Java. Go has no equivalent — there
        // you would register each handler explicitly in an init() function, the
        // way database/sql drivers call sql.Register.
        var handlers = AppDomain.CurrentDomain.GetAssemblies()
            // LINQ is Java's Stream API with different names:
            // SelectMany == flatMap, Where == filter, Select == map.
            .SelectMany(x => x.DefinedTypes)
            // Skip what could never be instantiated later: abstract classes,
            // interfaces, and open generics (List<T> with T unresolved — Java
            // erases generics, so it has no direct counterpart to this last one).
            .Where(x => !x.IsAbstract && !x.IsInterface && !x.IsGenericTypeDefinition)
            // Identical to Java's IPacketHandler.class.isAssignableFrom(x):
            // "is this class a subtype of IPacketHandler?". Watch the direction —
            // the *parent* is the receiver, the candidate is the argument.
            // Writing x.IsAssignableFrom(x) compiles and is always true, since
            // every class is a subtype of itself.
            //
            // Worth noting: in C# and Java implementing an interface is explicit
            // (`: IPacketHandler`), so this check has something to find. In Go
            // interfaces are satisfied structurally, just by having the methods,
            // and no such list of implementers exists to scan.
            .Where(x => typeof(IPacketHandler).IsAssignableFrom(x))
            // Read the annotation off the class. Java: t.getAnnotation(X.class).
            // Returns null when absent, hence the filter on the next line.
            // The (type: t, attr: ...) syntax is a tuple — an ad-hoc pair with
            // named fields, no class declaration needed. Java would need a
            // record or Map.Entry here; Go returns multiple values instead.
            .Select(t => (type: t, attr: t.GetCustomAttribute<HandlerRegisterAttribute>()))
            // A handler with no attribute opted out of the registry — a base
            // class or a test double, for instance.
            .Where(x => x.attr != null);

        foreach (var (type, attr) in handlers)
        {
            services.AddScoped(type);
        }

        return services;
    }

}