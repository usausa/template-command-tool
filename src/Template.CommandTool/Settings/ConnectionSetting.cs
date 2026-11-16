namespace Template.CommandTool.Settings;

public class ConnectionProfile
{
    public string? Host { get; set; }

    public int? Port { get; set; }

    public string? Key { get; set; }

    public string? Secret { get; set; }
}

public sealed class ConnectionSetting : ConnectionProfile
{
    public Dictionary<string, ConnectionProfile> Profiles { get; } = [];
}
