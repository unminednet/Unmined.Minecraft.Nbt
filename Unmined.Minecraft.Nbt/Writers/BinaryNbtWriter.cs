using System.Buffers.Binary;
using System.Text;
using Unmined.Minecraft.Nbt.Parsers;

namespace Unmined.Minecraft.Nbt.Writers;

public class BinaryNbtWriter : IDisposable
{
    private readonly bool _leaveOpen;

    private readonly byte[] _valueBuffer = new byte[8];

    public BinaryNbtWriter(BinaryWriter writer, BinaryNbtFormat format, bool leaveOpen = false)
    {
        _leaveOpen = leaveOpen;

        Writer = writer;
        Format = format;
    }

    public BinaryWriter Writer { get; }

    public BinaryNbtFormat Format { get; }

    public void WriteByteArray(ReadOnlySpan<byte> value)
    {
        InternalWriteArraySize(value.Length);
        Writer.Write(value);
    }

    public void WriteDouble(double value)
    {
        if (Format == BinaryNbtFormat.JavaEdition)
            BinaryPrimitives.WriteDoubleBigEndian(_valueBuffer, value);
        else
            BinaryPrimitives.WriteDoubleLittleEndian(_valueBuffer, value);

        Writer.Write(_valueBuffer.AsSpan().Slice(0, sizeof(double)));
    }

    public void WriteFloat(float value)
    {
        if (Format == BinaryNbtFormat.JavaEdition)
            BinaryPrimitives.WriteDoubleBigEndian(_valueBuffer, value);
        else
            BinaryPrimitives.WriteDoubleLittleEndian(_valueBuffer, value);

        Writer.Write(_valueBuffer.AsSpan().Slice(0, sizeof(float)));
    }

    public void WriteInt(int value)
    {
        Writer.Write(
            Format == BinaryNbtFormat.JavaEdition
                ? BinaryPrimitives.ReverseEndianness(value)
                : value);
    }

    public void WriteIntArray(ReadOnlySpan<int> value)
    {
        InternalWriteArraySize(value.Length);
        if (Format == BinaryNbtFormat.JavaEdition)
            foreach (var v in value)
                Writer.Write(BinaryPrimitives.ReverseEndianness(v));
        else
            foreach (var v in value)
                Writer.Write(v);
    }


    public void WriteListItemCount(int itemCount)
    {
        Writer.Write(
            Format == BinaryNbtFormat.JavaEdition
                ? BinaryPrimitives.ReverseEndianness(itemCount)
                : itemCount);
    }

    public void WriteListItemType(TagType itemType)
    {
        Writer.Write((byte)itemType);
    }

    public void WriteLong(long value)
    {
        Writer.Write(
            Format == BinaryNbtFormat.JavaEdition
                ? BinaryPrimitives.ReverseEndianness(value)
                : value);
    }

    public void WriteLongArray(ReadOnlySpan<long> value)
    {
        InternalWriteArraySize(value.Length);
        if (Format == BinaryNbtFormat.JavaEdition)
            foreach (var v in value)
                Writer.Write(BinaryPrimitives.ReverseEndianness(v));
        else
            foreach (var v in value)
                Writer.Write(v);
    }

    public void WriteSByte(sbyte value)
    {
        Writer.Write(value);
    }

    public void WriteShort(short value)
    {
        Writer.Write(
            Format == BinaryNbtFormat.JavaEdition
                ? BinaryPrimitives.ReverseEndianness(value)
                : value);
    }

    public void WriteString(ReadOnlySpan<char> value)
    {
        InternalWriteString(value);
    }

    public void WriteTagName(ReadOnlySpan<char> tagName)
    {
        InternalWriteString(tagName);
    }

    public void WriteTagType(TagType tagType)
    {
        Writer.Write((byte)tagType);
    }

    public void Dispose()
    {
        if (!_leaveOpen)
            Writer.Dispose();
    }

    private void InternalWriteArraySize(int size)
    {
        Writer.Write(
            Format == BinaryNbtFormat.JavaEdition
                ? BinaryPrimitives.ReverseEndianness(size)
                : size);
    }

    private void InternalWriteString(ReadOnlySpan<char> value)
    {
        if (value.Length == 0)
        {
            WriteStringLength(0);
            return;
        }

        var bytes = Encoding.UTF8.GetBytes(value.ToString());
        var length = (ushort)bytes.Length;

        WriteStringLength(length);
        Writer.Write(bytes);
    }

    private void WriteStringLength(ushort length)
    {
        Writer.Write(
            Format == BinaryNbtFormat.JavaEdition
                ? BinaryPrimitives.ReverseEndianness(length)
                : length);
    }
}