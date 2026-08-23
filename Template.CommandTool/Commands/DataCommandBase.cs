namespace Template.CommandTool.Commands;

using Smart.CommandLine.Hosting;

using Template.CommandTool.Settings;

public sealed record ConnectionParameter(string Host, int Port, string Key, string Secret);

public abstract class DataCommandBase
{
    private readonly ConnectionSetting setting;

    [Option<string>("--host", "-h", Description = "host (default: config)")]
    public string? Host { get; set; }

    [Option<int?>("--port", "-p", Description = "port (default: config)")]
    public int? Port { get; set; }

    [Option<string>("--key", "-k", Description = "private key (default: config)")]
    public string? Key { get; set; }

    [Option<string>("--secret", "-s", Description = "secret (default: config)")]
    public string? Secret { get; set; }

    [Option<OutputFormat>("--output", "-o", Description = "output format", DefaultValue = OutputFormat.Text)]
    public OutputFormat Output { get; set; }

    protected DataCommandBase(ConnectionSetting setting)
    {
        this.setting = setting;
    }

    protected ConnectionParameter? ResolveConnection()
    {
        var host = Host ?? setting.Host;
        var port = Port ?? setting.Port;
        var key = Key ?? setting.Key;
        var secret = Secret ?? setting.Secret;
        if (String.IsNullOrEmpty(host) || (port is null or 0) || String.IsNullOrEmpty(key) || String.IsNullOrEmpty(secret))
        {
            return null;
        }

        return new ConnectionParameter(host, port.Value, key, secret);
    }
}
