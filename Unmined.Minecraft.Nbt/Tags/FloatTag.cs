using System.Globalization;

namespace Unmined.Minecraft.Nbt.Tags;

public class FloatTag : Tag
{
    public FloatTag(float data)
    {
        Value = data;
    }

    public override TagType Type => TagType.Float;

    public float Value { get; set; }

    public override FloatTag Clone()
    {
        return new FloatTag(Value);
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}