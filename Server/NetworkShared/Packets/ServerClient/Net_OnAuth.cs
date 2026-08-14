using LiteNetLib.Utils;

public class Net_OnAuth : INetPacket
{
    public PacketType Type => PacketType.OnAuth;

    public string Username { get; set; }
    public int Score { get; set; }

    public void Deserialize(NetDataReader reader)
    {
        Username = reader.GetString();
        Score = reader.GetInt();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put((byte)Type);
        writer.Put(Username);
        writer.Put(Score);
    }
}