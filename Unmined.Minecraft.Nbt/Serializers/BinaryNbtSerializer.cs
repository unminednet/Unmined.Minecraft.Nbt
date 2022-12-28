using Unmined.Minecraft.Nbt.Tags;
using Unmined.Minecraft.Nbt.Writers;

namespace Unmined.Minecraft.Nbt.Serializers;

public static class BinaryNbtSerializer
{
    public static void Serialize(BinaryNbtWriter writer, CompoundTag rootTag)
    {
        writer.WriteTagType(TagType.Compound);
        writer.WriteTagName(rootTag is RootTag r ? r.Name : string.Empty);
        SerializeCompoundItems(writer, rootTag);
    }

    private static void SerializeCompoundItems(BinaryNbtWriter writer,
        IEnumerable<KeyValuePair<string, Tag>> tagsWithName)
    {
        foreach (var namedTag in tagsWithName)
        {
            writer.WriteTagType(namedTag.Value.Type);
            writer.WriteTagName(namedTag.Key);
            SerializePayload(writer, namedTag.Value);
        }

        writer.WriteTagType(TagType.End);
    }

    private static void SerializePayload(BinaryNbtWriter writer, Tag tag)
    {
        switch (tag.Type)
        {
            case TagType.Byte:
                writer.WriteSByte(((ByteTag)tag).Value);
                break;
            case TagType.Short:
                writer.WriteShort(((ShortTag)tag).Value);
                break;
            case TagType.Int:
                writer.WriteInt(((IntTag)tag).Value);
                break;
            case TagType.Long:
                writer.WriteLong(((LongTag)tag).Value);
                break;
            case TagType.Float:
                writer.WriteFloat(((FloatTag)tag).Value);
                break;
            case TagType.Double:
                writer.WriteDouble(((DoubleTag)tag).Value);
                break;
            case TagType.ByteArray:
                writer.WriteByteArray(((ByteArrayTag)tag).Value);
                break;
            case TagType.String:
                writer.WriteString(((StringTag)tag).Value);
                break;
            case TagType.List:
                var l = (ListTag)tag;
                writer.WriteListItemType(l.ItemType);
                writer.WriteListItemCount(l.Count);
                foreach (var t in l)
                    SerializePayload(writer, t);
                break;
            case TagType.Compound:
                var c = (CompoundTag)tag;
                SerializeCompoundItems(writer, c);
                break;
            case TagType.IntArray:
                writer.WriteIntArray(((IntArrayTag)tag).Value);
                break;
            case TagType.LongArray:
                writer.WriteLongArray(((LongArrayTag)tag).Value);
                break;
            default:
                throw new ArgumentOutOfRangeException($"Invalid tag type {tag.Type}");
        }
    }
}