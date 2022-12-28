namespace Unmined.Minecraft.Nbt;

public static class TagTypeValidator
{
    public static int MinValue = Enum.GetValues(typeof(TagType)).Cast<int>().Min();
    public static int MaxValue = Enum.GetValues(typeof(TagType)).Cast<int>().Max();

    public static bool IsValid(byte tagType)
    {
        return
            tagType >= MinValue
            && tagType <= MaxValue;
    }
}