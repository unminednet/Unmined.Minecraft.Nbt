using System.Buffers;
using System.Text;
using Unmined.Allocation;

namespace Unmined.Minecraft.Nbt.Parsers;

public class BinaryNbtStreamParser : BinaryNbtParserBase
{
    public const int MaxNameLength = 256;
    public const int DefaultDataBufferSize = 16384;

    private readonly BinaryReader _reader;
    private readonly byte[] _byteBuf = new byte[1];
    private readonly byte[] _nameBuffer;
    private byte[] _dataBuffer;

    public BinaryNbtStreamParser(Stream inputStream, BinaryNbtFormat format, bool leaveOpen = false) : base(format)
    {
        InputStream = inputStream;

        _reader = new BinaryReader(InputStream, Encoding.UTF8, leaveOpen);
        _nameBuffer = Allocator<byte>.Shared.Rent(MaxNameLength);
        _dataBuffer = Allocator<byte>.Shared.Rent(DefaultDataBufferSize);

        BeginRoot();
    }

    public override ReadOnlyMemory<byte> NameMemory => _nameBuffer.AsMemory(0, NameLength);
    public override ReadOnlyMemory<byte> DataMemory => _dataBuffer.AsMemory(0, DataLength);

    public Stream InputStream { get; }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _reader.Dispose();
            Allocator<byte>.Shared.Return(_nameBuffer);
            Allocator<byte>.Shared.Return(_dataBuffer);
        }
    }

    protected override void InternalLoadData()
    {
        if (DataLength > _dataBuffer.Length)
        {
            Allocator<byte>.Shared.Return(_dataBuffer);
            _dataBuffer = Allocator<byte>.Shared.Rent(DataLength);
        }

        _reader.BaseStream.ReadExactly(_dataBuffer, 0, DataLength);
    }

    protected override byte InternalReadByte()
    {
        _reader.BaseStream.ReadExactly(_byteBuf, 0, 1);

        return _byteBuf[0];
    }

    protected override int InternalReadInt()
    {
        return _reader.ReadInt32();
    }

    protected override short InternalReadShort()
    {
        return _reader.ReadInt16();
    }

    protected override void InternalReadTagName()
    {
        _reader.BaseStream.ReadExactly(_nameBuffer, 0, NameLength);
    }

    protected override void InternalSkip(int len)
    {
        InputStream.Skip(len);
    }
}