namespace Template.CommandTool.Commands;

using Smart.CommandLine.Hosting;

public abstract class DataCommandBase
{
    [Option<string>("--host", "-h", Description = "host", Required = true)]
    public required string Host { get; set; }

    [Option<int>("--port", "-p", Description = "port", DefaultValue = 18888)]
    public int Port { get; set; }

    [Option<string>("--key", "-k", Description = "private key", Required = true)]
    public required string Key { get; set; }

    [Option<string>("--secret", "-s", Description = "secret", Required = true)]
    public required string Secret { get; set; }
}
