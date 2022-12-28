namespace Unmined.Minecraft.Nbt.Parsers;

public interface IBufferedBinaryNbtParser : IBinaryNbtParser
{
    int InputPosition { get; }
    int NamePosition { get; }
    int NameLength { get; }
    ReadOnlyMemory<byte> NameMemory { get; }
    int DataPosition { get; }
    int DataLength { get; }
    ReadOnlyMemory<byte> DataMemory { get; }
    bool NameEquals(ReadOnlySpan<byte> tagNameUtf8);
}