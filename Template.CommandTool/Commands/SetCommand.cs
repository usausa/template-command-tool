namespace Template.CommandTool.Commands;

using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;

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

        public int Value { get; set; }

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
