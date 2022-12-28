namespace Unmined.Minecraft.Nbt.Tags;

public class IntArrayTag : Tag
{
    public IntArrayTag(int[] data)
    {
        Value = data;
    }

    public override TagType Type => TagType.IntArray;

    public int[] Value { get; set; }

    public override IntArrayTag Clone()
    {
        return new IntArrayTag(Value);
    }

    public override string? ToString()
    {
        return Value.ToString();
    }
}