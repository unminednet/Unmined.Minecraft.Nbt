using System.Buffers.Binary;

namespace Unmined.Minecraft.Nbt.Parsers;

public class BinaryNbtMemoryParser : BinaryNbtParserBase
{
    public BinaryNbtMemoryParser(ReadOnlyMemory<byte> inputBuffer, BinaryNbtFormat format) : base(format)
    {
        InputBuffer = inputBuffer;
        BeginRoot();
    }

    protected BinaryNbtMemoryParser() : base(BinaryNbtFormat.JavaEdition)
    {
    }

    public override ReadOnlyMemory<byte> NameMemory => InputBuffer.Slice(NamePosition, NameLength);
    public override ReadOnlyMemory<byte> DataMemory => InputBuffer.Slice(DataPosition, DataLength);

    public ReadOnlyMemory<byte> InputBuffer { get; private set; }

    protected virtual void Reset(ReadOnlyMemory<byte> inputBuffer, BinaryNbtFormat format)
    {
        base.Reset(format);

        InputBuffer = inputBuffer;
        BeginRoot();
    }

    protected override void Close()
    {
        base.Close();
        InputBuffer = null;
    }

    protected override void InternalLoadData()
    {
        // nothing to do
    }

    protected override byte InternalReadByte()
    {
        return InputBuffer.Span[InputPosition];
    }

    protected override int InternalReadInt()
    {
        return BinaryPrimitives.ReadInt32LittleEndian(InputBuffer.Span[InputPosition..]);
    }

    protected override short InternalReadShort()
    {
        return BinaryPrimitives.ReadInt16LittleEndian(InputBuffer.Span[InputPosition..]);
    }

    protected override void InternalReadTagName()
    {
        // nothing to do
    }


    protected override void InternalSkip(int len)
    {
        // nothing to do
    }
}