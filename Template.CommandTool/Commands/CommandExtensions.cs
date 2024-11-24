namespace Template.CommandTool.Commands;

using System.CommandLine;
using System.CommandLine.Hosting;

using Microsoft.Extensions.Hosting;

public static class CommandExtensions
{
    public static RootCommand Setup(this RootCommand command)
    {
        command.AddGlobalOption(new Option<string>(["--host", "-h"], "host") { IsRequired = true });
        command.AddGlobalOption(new Option<int>(["--port", "-p"], static () => 18888, "port"));
        command.AddGlobalOption(new Option<string>(["--key", "-k"], "private key") { IsRequired = true });
        command.AddGlobalOption(new Option<string>(["--secret", "-s"], "secret") { IsRequired = true });

        command.Add(new GetCommand());
        command.Add(new SetCommand());

        return command;
    }

    public static void UseCommandHandlers(this IHostBuilder host)
    {
        host.UseCommandHandler<GetCommand, GetCommand.CommandHandler>();
        host.UseCommandHandler<SetCommand, SetCommand.CommandHandler>();
    }
}
