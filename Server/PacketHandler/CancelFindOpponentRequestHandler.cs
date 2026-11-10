[HandlerRegister(PacketType.FindOpponentRequest)]
public class FindOpponentRequestHandler : IPacketHandler
{
    public void Handle(INetPacket packet, int connectionId)
    {
        Console.WriteLine("Receive find opponent request!");
    }
}