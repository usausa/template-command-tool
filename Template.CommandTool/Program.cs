using System.CommandLine.Builder;
using System.CommandLine;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;

using Template.CommandTool.Commands;

var rootCommand = new RootCommand("Command");
rootCommand.Setup();

var builder = new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .UseHost(host =>
    {
        host.ConfigureServices((_, _) =>
        {
            // TODO
        });

        host.UseCommandHandlers();
    });

return await builder.Build().InvokeAsync(args);
