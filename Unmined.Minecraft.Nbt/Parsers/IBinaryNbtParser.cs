namespace Unmined.Minecraft.Nbt.Parsers;

public interface IBinaryNbtParser : IDisposable
{
    BinaryNbtFormat Format { get; }
    TagType Type { get; }
    string Name { get; }
    TagType ListItemType { get; }
    int ArrayLength { get; }
    void BeginChildren();
    void ExpectTagType(TagType tagType);
    bool FetchNextSibling();
    string GetAsString();
    void GetByteArray(Span<byte> buffer);
    void GetData(Span<byte> buffer);
    double GetDouble();
    float GetFloat();
    int GetInt();
    void GetIntArray(Span<int> buffer);
    long GetLong();
    void GetLongArray(Span<long> buffer);
    sbyte GetSByte();
    short GetShort();
    string GetString();
}