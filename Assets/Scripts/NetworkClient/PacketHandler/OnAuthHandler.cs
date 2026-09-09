using System;
using UnityEngine;

namespace TTT.PacketHandlers
{
    [HandlerRegister(PacketType.OnAuth)]
    public class OnAuthHandler : IPacketHandler
    {
        public static event Action<Net_OnAuth> OnAuth;

        public void Handle(INetPacket packet, int connectionId)
        {
            var msg = (Net_OnAuth)packet;

            Debug.Log($"[Client] authenticated as '{msg.Username}' (score {msg.Score})");

            OnAuth?.Invoke(msg);
        }
    }
}
