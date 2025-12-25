namespace Template.CommandTool.Filters;

using Smart.CommandLine.Hosting;

public static class CommandExtensions
{
    public static void AddGlobalFilters(this ICommandBuilder command)
    {
        command.AddGlobalFilter<LoggingFilter>();
        command.AddGlobalFilter<ExceptionFilter>(Int32.MaxValue);
    }
}
