using System.Globalization;

namespace Unmined.Minecraft.Nbt.Tags;

public class ByteTag : Tag
{
    public ByteTag(sbyte data)
    {
        Value = data;
    }

    public override TagType Type => TagType.Byte;

    public sbyte Value { get; set; }

    public override ByteTag Clone()
    {
        return new ByteTag(Value);
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}