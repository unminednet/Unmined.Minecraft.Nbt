using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Unmined.Minecraft.Nbt.Parsers;
using Unmined.Minecraft.Nbt.Tags;
using Unmined.Minecraft.Nbt.Writers;

namespace Unmined.Minecraft.Nbt.Serializers;

public static class NbtConvert
{
    public static RootTag Deserialize(IBinaryNbtParser parser)
    {
        var result = BinaryNbtDeserializer.DeserializeRoot(parser);
        return result;
    }

    public static RootTag Deserialize(Stream stream, BinaryNbtFormat format, bool leaveOpen = false)
    {
        using var parser = new BinaryNbtStreamParser(stream, format, leaveOpen);
        return Deserialize(parser);
    }

    public static RootTag Deserialize(ReadOnlyMemory<byte> data, BinaryNbtFormat format)
    {
        using var parser = new BinaryNbtMemoryParser(data, format);
        return Deserialize(parser);
    }

    public static Tag Deserialize(ITextNbtParser parser)
    {
        return TextNbtDeserializer.Deserialize(parser);
    }

    public static Tag Deserialize(ReadOnlyMemory<char> text)
    {
        using var parser = new TextNbtMemoryParser(text);
        return Deserialize(parser);
    }

    public static Tag Deserialize(string text)
    {
        return Deserialize(text.AsMemory());
    }

    public static byte[] Serialize(CompoundTag tag, BinaryNbtFormat format)
    {
        using var stream = new MemoryStream();
        Serialize(tag, format, stream, true);
        return stream.ToArray();
    }

    public static void Serialize(CompoundTag tag, BinaryNbtFormat format, Stream stream, bool leaveOpen = false)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen);
        Serialize(tag, format, writer);
    }

    public static void Serialize(CompoundTag tag, BinaryNbtFormat format, BinaryWriter writer, bool leaveOpen = false)
    {
        using var nbtWriter = new BinaryNbtWriter(writer, format, leaveOpen);
        BinaryNbtSerializer.Serialize(nbtWriter, tag);
    }

    public static string Serialize(Tag tag, bool humanFriendly = false)
    {
        var stringWriter = new StringWriter(CultureInfo.InvariantCulture);
        using var writer = new TextNbtWriter(stringWriter, false, humanFriendly);
        TextNbtSerializer.Serialize(writer, tag);
        return stringWriter.ToString();
    }
}