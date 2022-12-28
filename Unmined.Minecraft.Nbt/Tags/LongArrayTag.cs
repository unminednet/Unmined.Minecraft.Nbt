namespace Unmined.Minecraft.Nbt.Tags;

public class LongArrayTag : Tag
{
    public LongArrayTag(long[] data)
    {
        Value = data;
    }

    public override TagType Type => TagType.LongArray;

    public long[] Value { get; set; }

    public override LongArrayTag Clone()
    {
        return new LongArrayTag(Value);
    }

    public override string? ToString()
    {
        return Value.ToString();
    }
}