namespace Template.CommandTool.Commands;

using Smart.CommandLine.Hosting;

using Template.CommandTool.Usecase;

[Command("get", "Get value")]
public sealed class DataGetCommand : DataCommandBase, ICommandHandler
{
    private readonly CommandUsecase commandUsecase;

    public DataGetCommand(CommandUsecase commandUsecase)
    {
        this.commandUsecase = commandUsecase;
    }

    public async ValueTask ExecuteAsync(CommandContext context)
    {
        await using var client = await commandUsecase.CreateClientWithAuthorizeAsync(Host, Port, Key, Secret);
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

        Console.WriteLine($"OK {value}");
    }
}
