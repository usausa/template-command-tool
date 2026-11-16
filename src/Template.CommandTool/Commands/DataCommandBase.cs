namespace Template.CommandTool.Commands;

using Smart.CommandLine.Hosting;

using Template.CommandTool.Settings;

public sealed record ConnectionParameter(string Host, int Port, string Key, string Secret);

public abstract class DataCommandBase
{
    private readonly ConnectionSetting setting;

    [Option<string>("--profile", "-c", Description = "connection profile (default: config)")]
    public string? Profile { get; set; }

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

    // 優先順はオプション > プロファイル > 既定(Connection)
    protected bool TryResolveConnection([NotNullWhen(true)] out ConnectionParameter? connection, [NotNullWhen(false)] out string? error)
    {
        connection = null;
        ConnectionProfile? profile = null;
        if (!String.IsNullOrEmpty(Profile) && !setting.Profiles.TryGetValue(Profile, out profile))
        {
            error = $"Connection profile not found. profile=[{Profile}]";
            return false;
        }

        var host = Host ?? profile?.Host ?? setting.Host;
        var port = Port ?? profile?.Port ?? setting.Port;
        var key = Key ?? profile?.Key ?? setting.Key;
        var secret = Secret ?? profile?.Secret ?? setting.Secret;
        if (String.IsNullOrEmpty(host) || (port is null or 0) || String.IsNullOrEmpty(key) || String.IsNullOrEmpty(secret))
        {
            error = "Connection parameter is missing.";
            return false;
        }

        connection = new ConnectionParameter(host, port.Value, key, secret);
        error = null;
        return true;
    }
}
