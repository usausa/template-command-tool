namespace Template.CommandTool.Components;

using System.Buffers.Text;

public static class BufferWriterExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteAndAdvance(this IBufferWriter<byte> writer, ReadOnlySpan<byte> value)
    {
        value.CopyTo(writer.GetSpan(value.Length));
        writer.Advance(value.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteAndAdvance(this IBufferWriter<byte> writer, int value)
    {
        var span = writer.GetSpan(11);
        Utf8Formatter.TryFormat(value, span, out var written);
        writer.Advance(written);
    }
}
