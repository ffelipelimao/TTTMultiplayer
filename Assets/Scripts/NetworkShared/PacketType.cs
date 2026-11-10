using LiteNetLib.Utils;

public enum PacketType : byte
{
    #region ClientServer
    Invalid = 0,
    AuthRequest = 1,
    ServerStatusRequest = 2,
    FindOpponentRequest = 3,
    CancelFindOpponentRequest = 4,
    #endregion

    #region ServerClient
    OnAuth = 100,
    OnAuthFailed = 101,
    OnServerStatus = 102,
    OnFindOpponent = 103,
    #endregion
}


public interface INetPacket : INetSerializable
{
    PacketType Type { get; }
}
