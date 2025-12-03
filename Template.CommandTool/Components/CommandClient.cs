namespace Template.CommandTool.Components;

using System.Buffers;
using System.Buffers.Text;

using Microsoft.AspNetCore.Connections;

using Smart.IO;
using Smart.Threading;

public sealed class CommandClient : IAsyncDisposable
{
    private const int Timeout = 60_000;

    private readonly ConnectionContext context;

    private readonly PooledBufferWriter<byte> receiveBuffer = new(1024);

    public CommandClient(ConnectionContext context)
    {
        this.context = context;
    }

    public async ValueTask DisposeAsync()
    {
        await context.Transport.Output.CompleteAsync().ConfigureAwait(false);
        await context.Transport.Input.CompleteAsync().ConfigureAwait(false);

        await context.DisposeAsync().ConfigureAwait(false);

        receiveBuffer.Dispose();
    }

    public async ValueTask<bool> ExitAsync()
    {
        // Exit
        context.Transport.Output.WriteAndAdvance("exit\r\n"u8);
        await context.Transport.Output.FlushAsync().ConfigureAwait(false);

        receiveBuffer.Clear();
        await ReceiveResponseAsync().ConfigureAwait(false);
        return IsSuccessResponse();
    }

    public async ValueTask<byte[]> ChallengeAsync()
    {
        // Challenge
        context.Transport.Output.WriteAndAdvance("challenge\r\n"u8);
        await context.Transport.Output.FlushAsync().ConfigureAwait(false);

        receiveBuffer.Clear();
        await ReceiveResponseAsync().ConfigureAwait(false);
        if (!IsSuccessResponse())
        {
            return [];
        }

        var index = receiveBuffer.WrittenSpan.IndexOf((byte)' ');
        if (index < 0)
        {
            return [];
        }

        return receiveBuffer.WrittenSpan[(index + 1)..].ToArray();
    }

    public async ValueTask<bool> AuthorizeAsync(ReadOnlyMemory<byte> signature)
    {
        // Authorize
        context.Transport.Output.WriteAndAdvance("authorize "u8);
        context.Transport.Output.WriteAndAdvance(signature.Span);
        context.Transport.Output.WriteAndAdvance("\r\n"u8);
        await context.Transport.Output.FlushAsync().ConfigureAwait(false);

        receiveBuffer.Clear();
        await ReceiveResponseAsync().ConfigureAwait(false);

        return IsSuccessResponse();
    }

    public async ValueTask<int?> GetAsync()
    {
        // Get
        context.Transport.Output.WriteAndAdvance("get\r\n"u8);
        await context.Transport.Output.FlushAsync().ConfigureAwait(false);

        receiveBuffer.Clear();
        await ReceiveResponseAsync().ConfigureAwait(false);

        if (!IsSuccessResponse())
        {
            return null;
        }

        if (!Utf8Parser.TryParse(ExtractResponseOption(), out int value, out _))
        {
            return null;
        }

        return value;
    }

    public async ValueTask<bool> SetAsync(int value)
    {
        // Set
        context.Transport.Output.WriteAndAdvance("set "u8);
        context.Transport.Output.WriteAndAdvance(value);
        context.Transport.Output.WriteAndAdvance("\r\n"u8);
        await context.Transport.Output.FlushAsync().ConfigureAwait(false);

        receiveBuffer.Clear();
        await ReceiveResponseAsync().ConfigureAwait(false);

        return IsSuccessResponse();
    }

    private bool IsSuccessResponse() => receiveBuffer.WrittenSpan.StartsWith("ok"u8);

    private ReadOnlySpan<byte> ExtractResponseOption() => receiveBuffer.WrittenSpan[3..];

    private async ValueTask ReceiveResponseAsync()
    {
        // Receive
        using var timeout = new ReusableCancellationTokenSource();
        while (true)
        {
            timeout.CancelAfter(Timeout);
            var result = await context.Transport.Input.ReadAsync(timeout.Token).ConfigureAwait(false);
            var buffer = result.Buffer;

            var read = false;
            if (!buffer.IsEmpty && ReadLine(ref buffer, out var line))
            {
                var length = (int)line.Length;
                line.CopyTo(receiveBuffer.GetSpan(length));
                receiveBuffer.Advance(length);
                read = true;
            }

            context.Transport.Input.AdvanceTo(buffer.Start, buffer.End);

            if (read || result.IsCompleted || result.IsCanceled)
            {
                break;
            }

            timeout.Reset();
        }
    }

    private static bool ReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
    {
        var reader = new SequenceReader<byte>(buffer);
        if (reader.TryReadTo(out ReadOnlySequence<byte> l, "\r\n"u8))
        {
            buffer = buffer.Slice(reader.Position);
            line = l;
            return true;
        }

        line = default;
        return false;
    }
}
