namespace Template.CommandTool.Commands;

using System.CommandLine.Invocation;

public abstract class BaseCommandHandler : ICommandHandler
{
    public required string Host { get; set; }

    public int Port { get; set; }

    public required string Key { get; set; }

    public required string Secret { get; set; }

    public int Invoke(InvocationContext context) => 0;

    public abstract Task<int> InvokeAsync(InvocationContext context);
}
