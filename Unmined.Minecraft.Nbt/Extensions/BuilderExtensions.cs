using Unmined.Minecraft.Nbt.Tags;

namespace Unmined.Minecraft.Nbt.Extensions;

public static class BuilderExtensions
{
    public static CompoundTag Add(this CompoundTag parent, string name, Action<CompoundTag> initializer)
    {
        var result = new CompoundTag();
        initializer(result);
        parent.Add(name, result);
        return parent;
    }

    public static CompoundTag Add(this CompoundTag parent, string name, float value)
    {
        var result = new FloatTag(value);
        parent.Add(name, result);
        return parent;
    }

    public static CompoundTag Add(this CompoundTag parent, string name, double value)
    {
        var result = new DoubleTag(value);
        parent.Add(name, result);
        return parent;
    }

    public static CompoundTag Add(this CompoundTag parent, string name, int value)
    {
        var result = new IntTag(value);
        parent.Add(name, result);
        return parent;
    }

    public static CompoundTag Add(this CompoundTag parent, string name, short value)
    {
        var result = new ShortTag(value);
        parent.Add(name, result);
        return parent;
    }

    public static CompoundTag Add(this CompoundTag parent, string name, sbyte value)
    {
        var result = new ByteTag(value);
        parent.Add(name, result);
        return parent;
    }

    public static CompoundTag Add(this CompoundTag parent, string name, string value)
    {
        var result = new StringTag(value);
        parent.Add(name, result);
        return parent;
    }

    public static CompoundTag Add(this CompoundTag parent, string name, int[] value)
    {
        var result = new IntArrayTag(value);
        parent.Add(name, result);
        return parent;
    }

    public static CompoundTag Add(this CompoundTag parent, string name, long[] value)
    {
        var result = new LongArrayTag(value);
        parent.Add(name, result);
        return parent;
    }

    public static CompoundTag Add(this CompoundTag parent, string name, byte[] value)
    {
        var result = new ByteArrayTag(value);
        parent.Add(name, result);
        return parent;
    }

    public static CompoundTag Add(this CompoundTag parent, string name, IEnumerable<string> items)
    {
        var result = new ListTag(TagType.String, items.Select(i => (Tag)new StringTag(i)));
        parent.Add(name, result);
        return parent;
    }

    public static CompoundTag Add(this CompoundTag parent, string name, IEnumerable<short> items)
    {
        var result = new ListTag(TagType.Short, items.Select(i => (Tag)new ShortTag(i)));
        parent.Add(name, result);
        return parent;
    }

    public static CompoundTag Add(this CompoundTag parent, string name, IEnumerable<int> items)
    {
        var result = new ListTag(TagType.Int, items.Select(i => (Tag)new IntTag(i)));
        parent.Add(name, result);
        return parent;
    }

    public static CompoundTag Add(this CompoundTag parent, string name, IEnumerable<long> items)
    {
        var result = new ListTag(TagType.Long, items.Select(i => (Tag)new LongTag(i)));
        parent.Add(name, result);
        return parent;
    }

    public static CompoundTag Add(this CompoundTag parent, string name, IEnumerable<float> items)
    {
        var result = new ListTag(TagType.Float, items.Select(i => (Tag)new FloatTag(i)));
        parent.Add(name, result);
        return parent;
    }

    public static CompoundTag Add(this CompoundTag parent, string name, IEnumerable<double> items)
    {
        var result = new ListTag(TagType.Double, items.Select(i => (Tag)new DoubleTag(i)));
        parent.Add(name, result);
        return parent;
    }

    public static CompoundTag Add(this CompoundTag parent, string name, IEnumerable<sbyte> items)
    {
        var result = new ListTag(TagType.Byte, items.Select(i => (Tag)new ByteTag(i)));
        parent.Add(name, result);
        return parent;
    }

    public static CompoundTag Add(this CompoundTag parent, string name, IEnumerable<CompoundTag> items)
    {
        var result = new ListTag(TagType.Compound, items);
        parent.Add(name, result);
        return parent;
    }
}