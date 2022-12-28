using System.Globalization;

namespace Unmined.Minecraft.Nbt.Tags;

public class ShortTag : Tag
{
    public ShortTag(short data)
    {
        Value = data;
    }

    public override TagType Type => TagType.Short;

    public short Value { get; set; }

    public override ShortTag Clone()
    {
        return new ShortTag(Value);
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}