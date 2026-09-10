namespace Template.CommandTool.Commands;

using Smart.CommandLine.Hosting;

using Template.CommandTool.Components;
using Template.CommandTool.Settings;
using Template.CommandTool.Usecase;

[Command("get", "Get value")]
public sealed class DataGetCommand : DataCommandBase, ICommandHandler
{
    private readonly CommandUsecase commandUsecase;

    public DataGetCommand(CommandUsecase commandUsecase, ConnectionSetting setting)
        : base(setting)
    {
        this.commandUsecase = commandUsecase;
    }

    public async ValueTask ExecuteAsync(CommandContext context)
    {
        var connection = ResolveConnection();
        if (connection is null)
        {
            Console.WriteLine("NG: Connection parameter is missing.");
            context.ExitCode = -1;
            return;
        }

        await using var client = await commandUsecase.CreateClientWithAuthorizeAsync(connection.Host, connection.Port, connection.Key, connection.Secret);
        if (client is null)
        {
            Console.WriteLine("NG: Authorize failed.");
            context.ExitCode = -1;
            return;
        }

        var value = await client.GetAsync();
        if (!value.HasValue)
        {
            Console.WriteLine("NG: Get failed.");
            context.ExitCode = -1;
            return;
        }

        OutputWriter.Write(Output, new { value.Value }, static x => $"OK {x.Value}");
    }
}
