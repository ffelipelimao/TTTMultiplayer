using LiteNetLib.Utils;

public struct Net_ServerStatusRequest : INetPacket
{
    public PacketType Type => PacketType.ServerStatusRequest;

    public void Deserialize(NetDataReader reader)
    {

    }

    public void Serialize(NetDataWriter writer)
    {

    }
}