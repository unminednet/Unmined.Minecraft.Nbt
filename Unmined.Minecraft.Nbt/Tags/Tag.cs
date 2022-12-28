namespace Unmined.Minecraft.Nbt.Tags;

public abstract class Tag
{
    public abstract TagType Type { get; }
    public abstract Tag Clone();
}