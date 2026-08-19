using LiteNetLib.Utils;

public class Net_OnAuthFailed : INetPacket
{
    public PacketType Type => PacketType.OnAuthFailed;

    public void Deserialize(NetDataReader reader)
    {

    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put((byte)Type);
    }
}