using Microsoft.Extensions.DependencyInjection;
using TTT.Server;
using TTT.Server.Infra;

// the DI to the => var server = NetworkServer();
var provider = Container.Configure();
var server = provider.GetRequiredService<NetworkServer>();

server.Start();

while (true)
{
    server.PollEvents();
    Thread.Sleep(15);
}
