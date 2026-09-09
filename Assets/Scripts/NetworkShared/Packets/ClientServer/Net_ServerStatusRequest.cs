using LiteNetLib.Utils;

public class Net_ServerStatusRequest : INetPacket
{
    public PacketType Type => PacketType.ServerStatusRequest;

    public void Deserialize(NetDataReader reader)
    {

    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put((byte)Type);
    }
}
