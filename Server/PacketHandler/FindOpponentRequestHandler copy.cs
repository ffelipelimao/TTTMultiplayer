[HandlerRegister(PacketType.CancelFindOpponentRequest)]
public class CancelFindOpponentRequestHandler : IPacketHandler
{
    public void Handle(INetPacket packet, int connectionId)
    {
        Console.WriteLine("Receive cancel find opponent request!");
    }
}