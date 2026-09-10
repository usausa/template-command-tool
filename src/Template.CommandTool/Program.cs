using BunnyTail.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Smart.CommandLine.Hosting;

using Template.CommandTool.Commands;
using Template.CommandTool.Components;
using Template.CommandTool.Filters;
using Template.CommandTool.Settings;
using Template.CommandTool.Usecase;

var builder = CommandHost.CreateBuilder(args)
    .UseDefaults();

builder.ConfigureContainer(new GeneratedServiceProviderFactory(static options => options.TrackTransientDisposables = false));

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddCommandComponents();
builder.Services.AddFilterComponents();

builder.Services.AddSingleton<CommandClientFactory>();
builder.Services.AddSingleton<CommandUsecase>();

builder.Services.AddSingleton(static p => p.GetRequiredService<IConfiguration>().GetSection("Connection").Get<ConnectionSetting>() ?? new ConnectionSetting());

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
#if DEBUG
if (host.Services is GeneratedServiceProvider generatedProvider)
{
    foreach (var line in BunnyTail.DependencyInjection.Diagnostics.ServiceFactoryReportExtensions.DescribeRuntimeFallbacks(generatedProvider).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
    {
        System.Diagnostics.Debug.WriteLine(line);
    }
}
#endif
return await host.RunAsync();
