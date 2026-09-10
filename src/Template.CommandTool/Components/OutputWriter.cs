namespace Template.CommandTool.Components;

using System.Text.Json;

using Template.CommandTool.Commands;

public static class OutputWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public static void Write<T>(OutputFormat format, T result, Func<T, string> text)
    {
        Console.WriteLine(format == OutputFormat.Json ? JsonSerializer.Serialize(result, SerializerOptions) : text(result));
    }
}
