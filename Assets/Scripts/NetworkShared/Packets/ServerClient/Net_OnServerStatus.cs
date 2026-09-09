using System.Security.Cryptography.X509Certificates;
using LiteNetLib.Utils;

public class Net_OnServerStatus : INetPacket
{
    public PacketType Type => PacketType.OnServerStatus;

    public ushort PlayersCount { get; set; }

    public Net_PlayerStatus[] Players { get; set; } = System.Array.Empty<Net_PlayerStatus>();

    public void Deserialize(NetDataReader reader)
    {
        PlayersCount = reader.GetUShort();

        var playersStatusLength = reader.GetUShort();

        Players = new Net_PlayerStatus[playersStatusLength];
        for (int i = 0; i < playersStatusLength; i++)
        {
            Players[i] = reader.Get<Net_PlayerStatus>();
        }
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put((byte)Type);
        writer.Put(PlayersCount);
        writer.Put((ushort)Players.Length);

        for (int i = 0; i < Players.Length; i++)
        {
            writer.Put(Players[i]);
        }
    }
}

public struct Net_PlayerStatus : INetSerializable
{
    public string Username { get; set; }

    public int Score { get; set; }

    public bool IsOnline { get; set; }

    public void Deserialize(NetDataReader reader)
    {
        Username = reader.GetString();
        Score = reader.GetInt();
        IsOnline = reader.GetBool();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Username);
        writer.Put(Score);
        writer.Put(IsOnline);
    }
}

