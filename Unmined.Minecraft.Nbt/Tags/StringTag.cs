namespace Unmined.Minecraft.Nbt.Tags;

public class StringTag : Tag
{
    public StringTag(string data)
    {
        Value = data;
    }

    public override TagType Type => TagType.String;

    public string Value { get; set; }

    public override StringTag Clone()
    {
        return new StringTag(Value);
    }

    public override string ToString()
    {
        return Value;
    }
}