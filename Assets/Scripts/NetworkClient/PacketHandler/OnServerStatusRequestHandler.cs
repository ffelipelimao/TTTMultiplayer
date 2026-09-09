using System;
using UnityEngine.SceneManagement;

[HandlerRegister(PacketType.OnServerStatus)]
public class OnServerStatusRequestHandler : IPacketHandler
{
    public static event Action<Net_OnServerStatus> OnServerStatus;
    public void Handle(INetPacket packet, int connectionId)
    {
        if (SceneManager.GetActiveScene().name != "01_Lobby")
        {
            return;
        }

        var msg = (Net_OnServerStatus)packet;
        OnServerStatus?.Invoke(msg);
    }
}