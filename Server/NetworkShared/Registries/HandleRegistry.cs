using System.Reflection;

/// <summary>
/// Maps each <see cref="PacketType"/> to the handler class responsible for
/// processing it — the sibling of <see cref="PacketRegistry"/>. One answers
/// "what object do these bytes become?", this one answers "who deals with it?".
///
/// Both replace a switch statement with a lookup table, but they discover their
/// entries differently, and the contrast is the interesting part:
///
///   PacketRegistry — the packet class itself declares its type (INetPacket.Type).
///                    The information lives in the class.
///   HandleRegistry — the handler is *labelled from the outside* with
///                    [HandlerRegister(PacketType.X)]. The information lives in
///                    metadata attached to the class.
///
/// A C# attribute is a Java annotation: [HandlerRegister(...)] is the same idea
/// as @Component or @EventListener. Go's nearest relative is the struct tag
/// (`json:"name"`) — declarative metadata a class carries and something else
/// reads back via reflection.
///
/// The payoff: adding a handler means writing the class and tagging it. No file
/// here, and no DI registration, has to be edited. Convention over configuration.
/// </summary>
public class HandleRegistry
{
    // Type is C#'s Class<?> (Java) / reflect.Type (Go). We store the *class*;
    // an instance is only created later, when a packet of that type arrives.
    private Dictionary<PacketType, Type> _handlers = new Dictionary<PacketType, Type>();

    /// <summary>
    /// The PacketType -> handler class map. Built on first access and cached,
    /// because the scan below walks every loaded class and is expensive.
    /// </summary>
    public Dictionary<PacketType, Type> Handlers
    {
        get
        {
            // Lazy init. Count == 0 doubles as "not built yet", which is fine
            // here but is *not* thread-safe: two threads hitting this at the
            // same time would both run Initialize(). Safe today only because
            // it warms up during startup, before clients connect.
            if (_handlers.Count == 0)
            {
                Initialize();
            }
            return _handlers;
        }
    }

    void Initialize()
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

        // Nothing above has executed yet. LINQ is lazy exactly like a Java
        // Stream: the query is a recipe, and the foreach is what runs it.
        foreach (var (type, attr) in handlers)
        {
            if (!_handlers.ContainsKey(attr.PacketType))
            {
                _handlers[attr.PacketType] = type;
            }
            else
            {
                throw new Exception($"Multiple handlers for `{attr.PacketType}` packet type!");
            }
        }
    }
}
