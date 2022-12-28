using System.Globalization;

namespace Unmined.Minecraft.Nbt.Tags;

public class LongTag : Tag
{
    public LongTag(long data)
    {
        Value = data;
    }

    public override TagType Type => TagType.Long;

    public long Value { get; set; }

    public override LongTag Clone()
    {
        return new LongTag(Value);
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}