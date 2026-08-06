using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TTT.Server.Infra;

public static class Container
{
    public static IServiceProvider Configure()
    {
        var services = new ServiceCollection();

        ConfigureServices(services);

        return services.BuildServiceProvider();
    }

    public static void ConfigureServices(IServiceCollection services)
    {

        services.AddLogging(builder => builder.AddSimpleConsole());
        services.AddSingleton<NetworkServer>();
        services.AddSingleton<PacketRegistry>();
        services.AddSingleton<HandleRegistry>();
        services.AddPacketHandlers();
    }

}
