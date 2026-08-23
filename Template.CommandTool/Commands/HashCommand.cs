namespace Template.CommandTool.Commands;

using System.Security.Cryptography;

using Smart.CommandLine.Hosting;

using Template.CommandTool.Components;

[Command("hash", "Calculate file hash")]
public sealed class HashCommand : ICommandHandler
{
    [Option<string>("--file", "-f", Description = "target file", Required = true)]
    public required string FilePath { get; set; }

    [Option<OutputFormat>("--output", "-o", Description = "output format", DefaultValue = OutputFormat.Text)]
    public OutputFormat Output { get; set; }

    public async ValueTask ExecuteAsync(CommandContext context)
    {
        if (!File.Exists(FilePath))
        {
            Console.WriteLine("NG: File not found.");
            context.ExitCode = -1;
            return;
        }

        await using var stream = File.OpenRead(FilePath);
        var hash = await SHA256.HashDataAsync(stream);

        OutputWriter.Write(Output, new { File = FilePath, Hash = Convert.ToHexString(hash) }, static x => $"OK {x.Hash}");
    }
}
