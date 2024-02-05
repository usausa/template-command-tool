using System.CommandLine.Builder;
using System.CommandLine;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;

using Microsoft.Extensions.DependencyInjection;

using Template.CommandTool.Commands;
using Template.CommandTool.Components;
using Template.CommandTool.Usecase;

var rootCommand = new RootCommand("Command");
rootCommand.Setup();

var builder = new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .UseHost(host =>
    {
        host.ConfigureServices((_, service) =>
        {
            service.AddSingleton<CommandClientFactory>();
            service.AddSingleton<CommandUsecase>();
        });

        host.UseCommandHandlers();
    });

return await builder.Build().InvokeAsync(args);
