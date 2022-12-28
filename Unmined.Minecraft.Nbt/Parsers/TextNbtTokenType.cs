namespace Unmined.Minecraft.Nbt.Parsers;

public enum TextNbtTokenType
{
    None,
    BeginCompound,
    EndCompound,
    BeginList,
    BeginArray,
    EndList,
    Separator,
    NameValueSeparator,
    Value
}