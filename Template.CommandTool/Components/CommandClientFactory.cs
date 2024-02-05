namespace Template.CommandTool.Components;

using System.Net;
using System.Net.Sockets;

using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Logging;

public sealed class CommandClientFactory : IDisposable
{
    private readonly SocketConnectionContextFactory connectionFactory;

    public CommandClientFactory(ILoggerFactory loggerFactory)
    {
        var options = new SocketConnectionFactoryOptions();
        connectionFactory = new SocketConnectionContextFactory(options, loggerFactory.CreateLogger<SocketConnectionContextFactory>());
    }

    public void Dispose()
    {
        connectionFactory.Dispose();
    }

#pragma warning disable CA2000
    public async ValueTask<CommandClient> CreateClientAsync(EndPoint endPoint)
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            NoDelay = true
        };

        try
        {
            await socket.ConnectAsync(endPoint).ConfigureAwait(false);

            return new CommandClient(connectionFactory.Create(socket));
        }
        catch
        {
            socket.Dispose();
            throw;
        }
    }
#pragma warning restore CA2000
}
