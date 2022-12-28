using Unmined.Minecraft.Nbt.Parsers;
using Unmined.Minecraft.Nbt.Tags;

namespace Unmined.Minecraft.Nbt.Serializers;

public static class BinaryNbtDeserializer
{
    public static RootTag DeserializeRoot(IBinaryNbtParser parser)
    {
        var tagName = parser.Name;
        var items = DeserializeCompoundItems(parser);
        var result = new RootTag(tagName, items);
        return result;
    }

    private static IEnumerable<KeyValuePair<string, Tag>> DeserializeCompoundItems(IBinaryNbtParser parser)
    {
        parser.BeginChildren();
        while (parser.FetchNextSibling())
            yield return new KeyValuePair<string, Tag>(parser.Name, DeserializeTag(parser));
    }

    private static IEnumerable<Tag> DeserializeListItems(IBinaryNbtParser parser)
    {
        parser.BeginChildren();
        while (parser.FetchNextSibling())
            yield return DeserializeTag(parser);
    }

    private static Tag DeserializeTag(IBinaryNbtParser parser)
    {
        return parser.Type switch
        {
            TagType.Byte => new ByteTag(parser.GetSByte()),
            TagType.Short => new ShortTag(parser.GetShort()),
            TagType.Int => new IntTag(parser.GetInt()),
            TagType.Long => new LongTag(parser.GetLong()),
            TagType.Float => new FloatTag(parser.GetFloat()),
            TagType.Double => new DoubleTag(parser.GetDouble()),
            TagType.String => new StringTag(parser.GetString()),
            TagType.List => new ListTag(parser.ListItemType, DeserializeListItems(parser)),
            TagType.Compound => new CompoundTag(DeserializeCompoundItems(parser)),
            TagType.ByteArray => new ByteArrayTag(GetByteArray(parser)),
            TagType.IntArray => new IntArrayTag(GetIntArray(parser)),
            TagType.LongArray => new LongArrayTag(GetLongArray(parser)),
            TagType.End => throw new InvalidDataException("Unexpected End tag"),
            _ => throw new InvalidDataException($"Unknown tag type {parser.Type}")
        };
    }

    private static byte[] GetByteArray(IBinaryNbtParser parser)
    {
        var array = new byte[parser.ArrayLength];
        parser.GetByteArray(array);
        return array;
    }

    private static int[] GetIntArray(IBinaryNbtParser parser)
    {
        var array = new int[parser.ArrayLength];
        parser.GetIntArray(array);
        return array;
    }

    private static long[] GetLongArray(IBinaryNbtParser parser)
    {
        var array = new long[parser.ArrayLength];
        parser.GetLongArray(array);
        return array;
    }
}