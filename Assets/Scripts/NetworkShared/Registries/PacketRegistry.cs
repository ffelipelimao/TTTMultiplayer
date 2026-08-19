using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Maps each <see cref="PacketType"/> to the class that models that packet on
/// the wire.
///
/// The problem it solves: a packet arrives as raw bytes, and the first byte is
/// the packet type. Something has to turn "byte 1" into "a Net_AuthRequest
/// object". The naive version is a growing switch:
///
///     switch (type) { case AuthRequest: return new Net_AuthRequest(); ... }
///
/// which every new packet forces you to edit. This registry replaces that
/// switch with a lookup table built at runtime — classic OO move: push the
/// decision out of a conditional and into polymorphism plus data.
/// </summary>
public class PacketRegistry
{
    // Type is C#'s Class<?> (Java) / reflect.Type (Go). The dictionary holds
    // the *class*, not an instance — instances are created per received packet.
    private Dictionary<PacketType, Type> _packetsType = new Dictionary<PacketType, Type>();

    /// <summary>
    /// The PacketType -> packet class map. Built on first access and cached,
    /// because the scan below walks every loaded class and is expensive.
    /// </summary>
    public Dictionary<PacketType, Type> PacketType
    {
        get
        {
            // Lazy init. Count == 0 doubles as "not built yet", which is fine
            // here but is *not* thread-safe: two threads hitting this at the
            // same time would both run Initialize(). Safe today only because
            // it warms up during startup, before clients connect.
            if (_packetsType.Count == 0)
            {
                Initialize();
            }
            return _packetsType;
        }
    }

    void Initialize()
    {
        var packetType = typeof(INetPacket);

        // Assembly == a compiled .dll, the rough equivalent of a .jar.
        // GetAssemblies() gives the ones already loaded into this process, so
        // this is a classpath scan: the same thing Spring's @ComponentScan or
        // the Reflections library does in Java. Go has no equivalent — there
        // you would register each packet explicitly in an init() function, the
        // way database/sql drivers call sql.Register.
        var packets = AppDomain.CurrentDomain.GetAssemblies()
            // LINQ is Java's Stream API with different names:
            // SelectMany == flatMap, Where == filter, Select == map.
            .SelectMany(s => s.GetTypes())
            // Identical to Java's IPacketHandler.class.isAssignableFrom(x):
            // "is this class a subtype of INetPacket?". Note the direction —
            // the *parent* is the receiver, the candidate is the argument.
            // Interfaces are excluded because INetPacket matches itself.
            .Where(packets => packetType.IsAssignableFrom(packets) && !packets.IsInterface);

        foreach (var packet in packets)
        {
            // Type is an instance property (PacketType Type => ...), so there
            // is no way to read it without an object. This throwaway instance
            // exists purely to ask the class "which type do you declare?".
            // Java equivalent: clazz.getDeclaredConstructor().newInstance().
            // It requires a parameterless constructor and throws if missing.
            var instance = (INetPacket)Activator.CreateInstance(packet);
            _packetsType.Add(instance.Type, packet);
        }

    }
}
