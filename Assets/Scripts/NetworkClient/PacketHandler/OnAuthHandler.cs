using System;
using UnityEngine;

namespace TTT.PacketHandlers
{
    /// <summary>
    /// Trata o <see cref="Net_OnAuth"/> — a resposta de sucesso que o servidor
    /// manda depois de validar o login (ver AuthRequestHandler no projeto Server).
    ///
    /// O atributo é o mesmo mecanismo do servidor: o HandleRegistry varre as
    /// assemblies e monta o mapa PacketType -> classe a partir dele. Nada
    /// precisa ser editado no Client para este handler passar a existir.
    /// </summary>
    [HandlerRegister(PacketType.OnAuth)]
    public class OnAuthHandler : IPacketHandler
    {
        // static porque o handler é criado por reflection: quem quer ouvir não
        // tem como pegar a instância. O evento vira o ponto de encontro entre a
        // camada de rede e a UI, sem que o Client conheça nenhuma das duas.
        public static event Action<Net_OnAuth> OnAuth;

        public void Handle(INetPacket packet, int connectionId)
        {
            var msg = (Net_OnAuth)packet;

            Debug.Log($"[Client] authenticated as '{msg.Username}' (score {msg.Score})");

            OnAuth?.Invoke(msg);
        }
    }
}
