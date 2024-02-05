namespace Template.CommandTool.Commands;

using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;

using Template.CommandTool.Usecase;

public sealed class SetCommand : Command
{
    public SetCommand()
        : base("set", "Set value")
    {
        AddOption(new Option<int>(["--value", "-v"], "value") { IsRequired = true });
    }

    public sealed class CommandHandler : BaseCommandHandler
    {
        private readonly IConsole console;

        private readonly CommandUsecase commandUsecase;

        public int Value { get; set; }

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

            if (!await client.SetAsync(Value))
            {
                console.Out.WriteLine("NG: Refresh failed.");
                return -1;
            }

            console.Out.WriteLine("OK");

            return 0;
        }
    }
}
