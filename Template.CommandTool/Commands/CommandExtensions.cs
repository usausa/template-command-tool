namespace Template.CommandTool.Commands;

using BunnyTail.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

using Smart.CommandLine.Hosting;

public static partial class CommandExtensions
{
    public static void AddCommands(this ICommandBuilder commands)
    {
        commands.AddCommand<DataCommand>(data =>
        {
            data.AddSubCommand<DataGetCommand>();
            data.AddSubCommand<DataSetCommand>();
        });

        commands.AddCommand<HashCommand>();
    }

    [ComponentRegistration(Lifetime.Transient, "Command$")]
    public static partial IServiceCollection AddCommandComponents(this IServiceCollection services);
}
