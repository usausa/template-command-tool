namespace Template.CommandTool.Commands;

using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;

using Template.CommandTool.Usecase;

public sealed class GetCommand : Command
{
    public GetCommand()
        : base("get", "Get value")
    {
    }

    public sealed class CommandHandler : BaseCommandHandler
    {
        private readonly IConsole console;

        private readonly CommandUsecase commandUsecase;

        public CommandHandler(IConsole console, CommandUsecase commandUsecase)
        {
            this.console = console;
            this.commandUsecase = commandUsecase;
        }

        public override async Task<int> InvokeAsync(InvocationContext context)
        {
            await using var client = await commandUsecase.CreateClientWithAuthorizeAsync(Host, Port, Key, Secret);
            if (client is null)
            {
                console.Out.WriteLine("NG: Authorize failed.");
                return -1;
            }

            var value = await client.GetAsync();
            if (!value.HasValue)
            {
                console.Out.WriteLine("NG: Get failed.");
                return -1;
            }

            console.Out.WriteLine($"OK {value}");

            return 0;
        }
    }
}
