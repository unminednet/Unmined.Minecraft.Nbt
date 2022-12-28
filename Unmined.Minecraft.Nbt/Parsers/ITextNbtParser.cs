namespace Unmined.Minecraft.Nbt.Parsers;

public interface ITextNbtParser : IDisposable
{
    public TextNbtTokenType TokenType { get; }
    public ReadOnlySpan<char> Token { get; }
    public int TokenPosition { get; }
    public bool MoveNext();
    public int InputPosition { get; }
}