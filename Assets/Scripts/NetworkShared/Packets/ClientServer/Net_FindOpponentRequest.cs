using LiteNetLib.Utils;

public struct Net_FindOpponentRequest : INetPacket
{
    public PacketType Type => PacketType.FindOpponentRequest;

    public void Deserialize(NetDataReader reader)
    {

    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put((byte)Type);
    }
}
