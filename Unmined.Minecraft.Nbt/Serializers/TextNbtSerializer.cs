using Unmined.Minecraft.Nbt.Extensions;
using Unmined.Minecraft.Nbt.Tags;
using Unmined.Minecraft.Nbt.Writers;

namespace Unmined.Minecraft.Nbt.Serializers;

public static class TextNbtSerializer
{
    public static void Serialize(TextNbtWriter writer, Tag tag)
    {
        switch (tag)
        {
            case ByteTag:
                writer.WriteSByte(tag.GetAsSByte());
                break;
            case IntTag:
                writer.WriteInt(tag.GetAsInt());
                break;
            case ShortTag:
                writer.WriteInt(tag.GetAsShort());
                break;
            case LongTag:
                writer.WriteLong(tag.GetAsLong());
                break;
            case DoubleTag:
                writer.WriteDouble(tag.GetAsDouble());
                break;
            case FloatTag:
                writer.WriteFloat(tag.GetAsFloat());
                break;
            case StringTag:
                writer.WriteString(tag.GetAsString());
                break;
            case ByteArrayTag b:
                writer.WriteByteArray(b.Value);
                break;
            case IntArrayTag i:
                writer.WriteIntArray(i.Value);
                break;
            case LongArrayTag l:
                writer.WriteLongArray(l.Value);
                break;
            case RootTag r:
                writer.BeginRoot(r.Name);
                var isFirstRootItemWritten = false;
                foreach (var t in r)
                {
                    if (isFirstRootItemWritten)
                        writer.WriteCompoundSeparator();
                    else
                        isFirstRootItemWritten = true;
                    writer.BeginCompoundItem();
                    writer.WriteName(t.Key);
                    Serialize(writer, t.Value);
                    writer.EndCompoundItem();
                }

                writer.EndRoot();
                break;
            case CompoundTag c:
                writer.BeginCompound();
                var isFirstCompoundItemWritten = false;
                foreach (var t in c)
                {
                    if (isFirstCompoundItemWritten)
                        writer.WriteCompoundSeparator();
                    else
                        isFirstCompoundItemWritten = true;

                    writer.BeginCompoundItem();
                    writer.WriteName(t.Key);
                    Serialize(writer, t.Value);
                    writer.EndCompoundItem();
                }

                writer.EndCompound();
                break;
            case ListTag n:
                writer.BeginList();
                var isFirstListItemWritten = false;
                foreach (var t in n)
                {
                    if (isFirstListItemWritten)
                        writer.WriteListSeparator();
                    else
                        isFirstListItemWritten = true;

                    writer.BeginListItem();
                    Serialize(writer, t);
                    writer.EndListItem();
                }

                writer.EndList();
                break;
            default:
                throw new InvalidOperationException($"Unknown tag type {tag.Type}");
        }
    }
}