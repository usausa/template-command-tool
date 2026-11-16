namespace Template.CommandTool.Components;

using System.Globalization;
using System.Text.Json;

using Template.CommandTool.Commands;

public static class OutputWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private static readonly SearchValues<char> CsvSpecialChars = SearchValues.Create(",\"\r\n");

    public static void Write<T>(OutputFormat format, T result, Func<T, string> text)
    {
        Console.WriteLine(format switch
        {
            OutputFormat.Json => JsonSerializer.Serialize(result, SerializerOptions),
            OutputFormat.Csv => ToCsv(result),
            _ => text(result)
        });
    }

    // Text keeps the NG line. Json / Csv keep the same shape as a success so that the caller can parse it
    public static void WriteError(OutputFormat format, string message) =>
        Write(format, new { Success = false, Message = message }, static x => $"NG: {x.Message}");

    // Header + one row. Column names follow the JSON naming
    private static string ToCsv<T>(T result)
    {
        var properties = typeof(T).GetProperties();
        var header = String.Join(',', properties.Select(static x => Escape(SerializerOptions.PropertyNamingPolicy!.ConvertName(x.Name))));
        var row = String.Join(',', properties.Select(x => Escape(ToText(x.GetValue(result)))));
        return header + Environment.NewLine + row;
    }

    private static string? ToText(object? value) =>
        value switch
        {
            bool b => b ? "true" : "false",
            _ => Convert.ToString(value, CultureInfo.InvariantCulture)
        };

    private static string Escape(string? value)
    {
        if (String.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value.AsSpan().IndexOfAny(CsvSpecialChars) >= 0 ? $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\"" : value;
    }
}
