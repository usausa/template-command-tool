using System.CommandLine.Builder;
using System.CommandLine;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;

var rootCommand = new RootCommand("Command");
// TODO

var builder = new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .UseHost(host =>
    {
        host.ConfigureServices((_, _) =>
        {
            // TODO
        });

        // TODO
    });

return await builder.Build().InvokeAsync(args);
