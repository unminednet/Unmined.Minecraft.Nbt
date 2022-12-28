using System.Globalization;

namespace Unmined.Minecraft.Nbt.Tags;

public class DoubleTag : Tag
{
    public DoubleTag(double data)
    {
        Value = data;
    }

    public override TagType Type => TagType.Double;

    public double Value { get; set; }

    public override DoubleTag Clone()
    {
        return new DoubleTag(Value);
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}