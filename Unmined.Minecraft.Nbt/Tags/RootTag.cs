namespace Unmined.Minecraft.Nbt.Tags;

public class RootTag : CompoundTag
{
    public RootTag()
    {
    }

    public RootTag(string? name)
    {
        Name = name;
    }

    public RootTag(string? name, IEnumerable<KeyValuePair<string, Tag>> tags) : base(tags)
    {
        Name = name;
    }

    public RootTag(IEnumerable<KeyValuePair<string, Tag>> tags) : base(tags)
    {
    }

    public string? Name { get; set; }

    public override RootTag Clone()
    {
        return new RootTag(Name, this);
    }
}