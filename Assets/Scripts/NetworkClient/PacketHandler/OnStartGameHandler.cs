using UnityEngine;
using UnityEngine.SceneManagement;

namespace TTT.PacketHandlers
{
    [HandlerRegister(PacketType.OnStartMatch)]
    public class OnStartGameHandler : IPacketHandler
    {
        private const string GameScene = "02_Game";

        public void Handle(INetPacket packet, int connectionId)
        {
            var msg = (Net_OnStartGame)packet;

            Debug.Log($"[Client] game {msg.GameId} started: {msg.XUser} (X) vs {msg.OUser} (O)");

            SceneManager.LoadScene(GameScene);
        }
    }
}
