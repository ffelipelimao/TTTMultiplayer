using System;

[AttributeUsage(AttributeTargets.Class)]
public class HandlerRegisterAttribute : Attribute
{
    public HandlerRegisterAttribute(PacketType packetType)
    {
        PacketType = packetType;
    }

    public PacketType PacketType { get; set; }
}
