using System.Buffers.Binary;
using System.Globalization;
using System.Text;
using Unmined.Minecraft.Nbt.Parsers;
using Unmined.Minecraft.Nbt.Tags;

namespace Unmined.Minecraft.Nbt.Extensions;

public static class DataAccessExtensions
{
    public static bool GetAsBool(this Tag tag)
    {
        return tag.Type switch
        {
            TagType.String => Convert.ToBoolean(((StringTag)tag).Value,
                CultureInfo.InvariantCulture),
            TagType.Byte => Convert.ToBoolean(((ByteTag)tag).Value),
            TagType.Double => Convert.ToBoolean(((DoubleTag)tag).Value),
            TagType.Float => Convert.ToBoolean(((FloatTag)tag).Value),
            TagType.Int => Convert.ToBoolean(((IntTag)tag).Value),
            TagType.Long => Convert.ToBoolean(((LongTag)tag).Value),
            TagType.Short => Convert.ToBoolean(((ShortTag)tag).Value),
            _ => throw new NotSupportedException($"Cannot read tag type {tag.GetType().Name} as float.")
        };
    }

    public static byte GetAsByte(this Tag tag)
    {
        return tag.Type switch
        {
            TagType.Byte => unchecked((byte)((ByteTag)tag).Value),
            _ => throw new NotSupportedException($"Cannot read tag type {tag.GetType().Name} as byte.")
        };
    }

    public static byte[] GetAsByteArray(this Tag tag)
    {
        return tag.Type switch
        {
            TagType.List => ((ListTag)tag).Select(n => n.GetAsByte()).ToArray(),
            TagType.ByteArray => ((ByteArrayTag)tag).Value.ToArray(),
            _ => throw new NotSupportedException($"Cannot read tag type {tag.GetType().Name} as byte array.")
        };
    }

    public static double GetAsDouble(this Tag tag)
    {
        return tag.Type switch
        {
            TagType.String => Convert.ToDouble(((StringTag)tag).Value,
                CultureInfo.InvariantCulture),
            TagType.Byte => ((ByteTag)tag).Value,
            TagType.Double => ((DoubleTag)tag).Value,
            TagType.Float => ((FloatTag)tag).Value,
            TagType.Int => ((IntTag)tag).Value,
            TagType.Long => ((LongTag)tag).Value,
            TagType.Short => ((ShortTag)tag).Value,
            _ => throw new NotSupportedException($"Cannot read tag type {tag.GetType().Name} as double.")
        };
    }

    public static float GetAsFloat(this Tag tag)
    {
        return tag.Type switch
        {
            TagType.String => Convert.ToSingle(((StringTag)tag).Value,
                CultureInfo.InvariantCulture),
            TagType.Byte => ((ByteTag)tag).Value,
            TagType.Double => (float)((DoubleTag)tag).Value,
            TagType.Float => ((FloatTag)tag).Value,
            TagType.Int => ((IntTag)tag).Value,
            TagType.Long => ((LongTag)tag).Value,
            TagType.Short => ((ShortTag)tag).Value,
            _ => throw new NotSupportedException($"Cannot read tag type {tag.GetType().Name} as float.")
        };
    }

    public static int GetAsInt(this Tag tag)
    {
        return tag.Type switch
        {
            TagType.String => Convert.ToInt32(((StringTag)tag).Value, CultureInfo.InvariantCulture),
            TagType.Byte => ((ByteTag)tag).Value,
            TagType.Int => ((IntTag)tag).Value,
            TagType.Long => (int)((LongTag)tag).Value,
            TagType.Short => ((ShortTag)tag).Value,
            _ => throw new NotSupportedException($"Cannot read tag type {tag.GetType().Name} as int.")
        };
    }

    public static int[] GetAsIntArray(this Tag tag)
    {
        return tag.Type switch
        {
            TagType.List => ((ListTag)tag).Select(n => n.GetAsInt()).ToArray(),
            TagType.IntArray => ((IntArrayTag)tag).Value.ToArray(),
            _ => throw new NotSupportedException($"Cannot read tag type {tag.GetType().Name} as int array.")
        };
    }

    public static long GetAsLong(this Tag tag)
    {
        return tag.Type switch
        {
            TagType.String => Convert.ToInt64(((StringTag)tag).Value, CultureInfo.InvariantCulture),
            TagType.Byte => ((ByteTag)tag).Value,
            TagType.Int => ((IntTag)tag).Value,
            TagType.Long => ((LongTag)tag).Value,
            TagType.Short => ((ShortTag)tag).Value,
            _ => throw new NotSupportedException($"Cannot read tag type {tag.GetType().Name} as long.")
        };
    }

    public static long[] GetAsLongArray(this Tag tag)
    {
        return tag.Type switch
        {
            TagType.List => ((ListTag)tag).Select(n => n.GetAsLong()).ToArray(),
            TagType.LongArray => ((LongArrayTag)tag).Value.ToArray(),
            _ => throw new NotSupportedException($"Cannot read tag type {tag.GetType().Name} as long array.")
        };
    }

    public static sbyte GetAsSByte(this Tag tag)
    {
        return tag.Type switch
        {
            TagType.Byte => ((ByteTag)tag).Value,
            _ => throw new NotSupportedException($"Cannot read tag type {tag.GetType().Name} as sbyte.")
        };
    }

    public static short GetAsShort(this Tag tag)
    {
        return tag.Type switch
        {
            TagType.Byte => ((ByteTag)tag).Value,
            TagType.Short => ((ShortTag)tag).Value,
            _ => throw new NotSupportedException($"Cannot read tag type {tag.GetType().Name} as short.")
        };
    }

    public static string GetAsString(ReadOnlySpan<byte> data, TagType type, BinaryNbtFormat format)
    {
        switch (format)
        {
            case BinaryNbtFormat.JavaEdition:
                return type switch
                {
                    TagType.Byte => ((sbyte)data[0]).ToString(CultureInfo.InvariantCulture),
                    TagType.Short => BinaryPrimitives.ReadInt16BigEndian(data).ToString(CultureInfo.InvariantCulture),
                    TagType.Int => BinaryPrimitives.ReadInt32BigEndian(data).ToString(CultureInfo.InvariantCulture),
                    TagType.Long => BinaryPrimitives.ReadInt64BigEndian(data).ToString(CultureInfo.InvariantCulture),
                    TagType.Float => BinaryPrimitives.ReadSingleBigEndian(data).ToString(CultureInfo.InvariantCulture),
                    TagType.Double => BinaryPrimitives.ReadDoubleBigEndian(data).ToString(CultureInfo.InvariantCulture),
                    TagType.String => Encoding.UTF8.GetString(data),
                    _ => throw new InvalidOperationException($"Cannot convert tag data of type {type} to string")
                };
            case BinaryNbtFormat.BedrockEdition:
                return type switch
                {
                    TagType.Byte => ((sbyte)data[0]).ToString(CultureInfo.InvariantCulture),
                    TagType.Short => BinaryPrimitives.ReadInt16LittleEndian(data)
                        .ToString(CultureInfo.InvariantCulture),
                    TagType.Int => BinaryPrimitives.ReadInt32LittleEndian(data).ToString(CultureInfo.InvariantCulture),
                    TagType.Long => BinaryPrimitives.ReadInt64LittleEndian(data).ToString(CultureInfo.InvariantCulture),
                    TagType.Float => BinaryPrimitives.ReadSingleLittleEndian(data)
                        .ToString(CultureInfo.InvariantCulture),
                    TagType.Double => BinaryPrimitives.ReadDoubleLittleEndian(data)
                        .ToString(CultureInfo.InvariantCulture),
                    TagType.String => Encoding.UTF8.GetString(data),
                    _ => throw new InvalidOperationException($"Cannot convert tag data of type {type} to string")
                };
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }

    public static string GetAsString(this Tag tag)
    {
        return tag.Type switch
        {
            TagType.String => ((StringTag)tag).Value,
            TagType.Byte => ((ByteTag)tag).Value.ToString(CultureInfo.InvariantCulture),
            TagType.Double => ((DoubleTag)tag).Value.ToString(CultureInfo.InvariantCulture),
            TagType.Float => ((FloatTag)tag).Value.ToString(CultureInfo.InvariantCulture),
            TagType.Int => ((IntTag)tag).Value.ToString(CultureInfo.InvariantCulture),
            TagType.Long => ((LongTag)tag).Value.ToString(CultureInfo.InvariantCulture),
            TagType.Short => ((ShortTag)tag).Value.ToString(CultureInfo.InvariantCulture),
            _ => throw new NotSupportedException($"Cannot read tag type {tag.GetType().Name} as string.")
        };
    }

    public static bool IsArrayType(this Tag tag)
    {
        return tag.Type switch
        {
            TagType.IntArray
                or TagType.LongArray
                or TagType.ByteArray => true,
            _ => false
        };
    }

    public static bool IsDataType(this Tag tag)
    {
        return IsValueType(tag) || IsArrayType(tag);
    }

    public static bool IsValueType(this Tag tag)
    {
        return tag.Type switch
        {
            TagType.String
                or TagType.Byte
                or TagType.Double
                or TagType.Float
                or TagType.Int
                or TagType.Long
                or TagType.Short => true,
            _ => false
        };
    }
}