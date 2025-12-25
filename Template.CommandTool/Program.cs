using Microsoft.Extensions.DependencyInjection;

using Smart.CommandLine.Hosting;

using Template.CommandTool.Commands;
using Template.CommandTool.Components;
using Template.CommandTool.Filters;
using Template.CommandTool.Usecase;

var builder = CommandHost.CreateBuilder(args)
    .UseDefaults();

builder.Services.AddSingleton<CommandClientFactory>();
builder.Services.AddSingleton<CommandUsecase>();

builder.ConfigureCommands(commands =>
{
    commands.ConfigureRootCommand(root =>
    {
        root.WithDescription("Template");
    });

    commands.AddGlobalFilters();
    commands.AddCommands();
});

var host = builder.Build();
return await host.RunAsync();
