using LiteNetLib.Utils;

public enum PacketType : byte
{
    #region ClientServer
    Invalid = 0,
    AuthRequest = 1,
    #endregion

    #region ServerClient
    OnAuth = 100,
    OnAuthFailed = 101,
    OnServerStatus = 102,
    #endregion
}


public interface INetPacket : INetSerializable
{
    PacketType Type { get; }
}
