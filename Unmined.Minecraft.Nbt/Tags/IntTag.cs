using System.Globalization;

namespace Unmined.Minecraft.Nbt.Tags;

public class IntTag : Tag
{
    public IntTag(int data)
    {
        Value = data;
    }

    public override TagType Type => TagType.Int;

    public int Value { get; set; }

    public override IntTag Clone()
    {
        return new IntTag(Value);
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}