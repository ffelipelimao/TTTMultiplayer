using LiteNetLib.Utils;

public class Net_OnServerStatus : INetPacket
{
    public PacketType Type => PacketType.OnServerStatus;

    // TODO: implement fully - carregar a lista de jogadores conectados.

    public void Deserialize(NetDataReader reader)
    {

    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put((byte)Type);
    }
}
