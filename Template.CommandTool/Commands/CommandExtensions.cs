namespace Template.CommandTool.Commands;

using Smart.CommandLine.Hosting;

public static class CommandExtensions
{
    public static void AddCommands(this ICommandBuilder commands)
    {
        commands.AddCommand<DataCommand>(data =>
        {
            data.AddSubCommand<DataGetCommand>();
            data.AddSubCommand<DataSetCommand>();
        });
    }
}
