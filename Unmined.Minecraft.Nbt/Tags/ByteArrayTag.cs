namespace Unmined.Minecraft.Nbt.Tags;

public class ByteArrayTag : Tag
{
    public ByteArrayTag(byte[] data)
    {
        Value = data;
    }

    public override TagType Type => TagType.ByteArray;

    public byte[] Value { get; set; }


    public override ByteArrayTag Clone()
    {
        return new ByteArrayTag(Value);
    }

    public override string? ToString()
    {
        return Value.ToString();
    }
}