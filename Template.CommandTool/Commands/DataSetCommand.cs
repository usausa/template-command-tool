namespace Template.CommandTool.Commands;

using Smart.CommandLine.Hosting;

using Template.CommandTool.Components;
using Template.CommandTool.Settings;
using Template.CommandTool.Usecase;

[Command("set", "Set value")]
public sealed class DataSetCommand : DataCommandBase, ICommandHandler
{
    private readonly CommandUsecase commandUsecase;

    [Option("--value", "-v", "value", Required = true)]
    public int Value { get; set; }

    public DataSetCommand(CommandUsecase commandUsecase, ConnectionSetting setting)
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

        if (!await client.SetAsync(Value))
        {
            Console.WriteLine("NG: Set failed.");
            context.ExitCode = -1;
            return;
        }

        OutputWriter.Write(Output, new { Success = true, Value }, static x => $"OK {x.Value}");
    }
}
