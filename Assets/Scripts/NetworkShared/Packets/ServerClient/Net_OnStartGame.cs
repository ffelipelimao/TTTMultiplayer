using System;
using LiteNetLib.Utils;

public class Net_OnStartGame : INetPacket
{
    public PacketType Type => PacketType.OnStartMatch;

    public string XUser { get; set; }
    public string OUser { get; set; }
    public Guid GameId { get; set; }

    public void Deserialize(NetDataReader reader)
    {
        XUser = reader.GetString();
        OUser = reader.GetString();
        GameId = Guid.Parse(reader.GetString());
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put((byte)Type);
        writer.Put(XUser);
        writer.Put(OUser);
        writer.Put(GameId.ToString());
    }
}