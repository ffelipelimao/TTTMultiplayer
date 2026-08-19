using System;
using UnityEngine;

namespace TTT.PacketHandlers
{

    [HandlerRegister(PacketType.OnAuthFailed)]
    public class OnAuthFailedHandler : IPacketHandler
    {
        public static event Action<Net_OnAuthFailed> OnAuthFailed;

        public void Handle(INetPacket packet, int connectionId)
        {
            var msg = (Net_OnAuthFailed)packet;

            Debug.LogWarning("[Client] authentication refused by server");

            OnAuthFailed?.Invoke(msg);
        }
    }
}
