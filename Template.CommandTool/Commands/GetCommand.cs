namespace Template.CommandTool.Commands;

using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;

public sealed class GetCommand : Command
{
    public GetCommand()
        : base("get", "Get value")
    {
    }

    public sealed class CommandHandler : BaseCommandHandler
    {
        private readonly IConsole console;

        public CommandHandler(IConsole console)
        {
            this.console = console;
        }

        public override Task<int> InvokeAsync(InvocationContext context)
        {
            console.Out.WriteLine("TODO implement");

            return Task.FromResult(0);
        }
    }
}
