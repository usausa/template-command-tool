namespace Template.CommandTool.Filters;

using Microsoft.Extensions.Logging;

using Smart.CommandLine.Hosting;

public sealed class LoggingFilter : ICommandFilter
{
    private readonly ILogger<LoggingFilter> log;

    public LoggingFilter(ILogger<LoggingFilter> log)
    {
        this.log = log;
    }

    public async ValueTask ExecuteAsync(CommandContext context, CommandDelegate next)
    {
        log.InfoCommandStart(context.CommandType.Name);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            log.InfoCommandEnd(context.CommandType.Name, stopwatch.ElapsedMilliseconds);
        }
    }
}
