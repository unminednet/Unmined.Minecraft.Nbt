namespace Unmined.Minecraft.Nbt.Parsers;

public class TextNbtMemoryParser : ITextNbtParser
{
    public TextNbtMemoryParser(ReadOnlyMemory<char> input)
    {
        Input = input;
        TokenType = TextNbtTokenType.None;
        TokenPosition = 0;
        InputPosition = 0;
        TokenMemory = ReadOnlyMemory<char>.Empty;
    }

    public ReadOnlyMemory<char> TokenMemory { get; private set; }

    public ReadOnlyMemory<char> Input { get; }

    public TextNbtTokenType TokenType { get; private set; }

    public ReadOnlySpan<char> Token => TokenMemory.Span;

    public int TokenPosition { get; private set; }

    public int InputPosition { get; private set; }

    private static bool IsSymbol(char c)
    {
        return c is '{' or '}' or ',' or ';' or ':' or '[' or ']';
    }

    public void Dispose()
    {
        // nothing to dispose
    }

    public bool MoveNext()
    {
        while (InputPosition < Input.Length && char.IsWhiteSpace(Input.Span[InputPosition]))
            InputPosition++;

        if (InputPosition >= Input.Length)
        {
            TokenPosition = Input.Length;
            TokenType = TextNbtTokenType.None;
            TokenMemory = ReadOnlyMemory<char>.Empty;
            return false;
        }

        TokenPosition = InputPosition;

        var firstChar = Input.Span[InputPosition];

        switch (firstChar)
        {
            case '{':
                TokenType = TextNbtTokenType.BeginCompound;
                TokenMemory = Input.Slice(TokenPosition, 1);
                InputPosition++;
                return true;
            case '}':
                TokenType = TextNbtTokenType.EndCompound;
                TokenMemory = Input.Slice(TokenPosition, 1);
                InputPosition++;
                return true;
            case '[':
                if (InputPosition + 2 < Input.Length && Input.Span[InputPosition + 2] == ';')
                {
                    TokenMemory = Input.Slice(TokenPosition, 3);
                    TokenType = TextNbtTokenType.BeginArray;
                    InputPosition += 3;
                    return true;
                }

                TokenType = TextNbtTokenType.BeginList;
                TokenMemory = Input.Slice(TokenPosition, 1);
                InputPosition++;
                return true;
            case ']':
                TokenType = TextNbtTokenType.EndList;
                TokenMemory = Input.Slice(TokenPosition, 1);
                InputPosition++;
                return true;
            case ',':
                TokenType = TextNbtTokenType.Separator;
                TokenMemory = Input.Slice(TokenPosition, 1);
                InputPosition++;
                return true;
            case ':':
                TokenType = TextNbtTokenType.NameValueSeparator;
                TokenMemory = Input.Slice(TokenPosition, 1);
                InputPosition++;
                return true;
            case '"' or '\'':
            {
                TokenType = TextNbtTokenType.Value;
                InputPosition++;
                while (InputPosition < Input.Length)
                {
                    if (Input.Span[InputPosition] == firstChar)
                    {
                        InputPosition++;
                        TokenMemory = Input.Slice(TokenPosition, InputPosition - TokenPosition);
                        return true;
                    }

                    if (Input.Span[InputPosition] == '\\')
                    {
                        if (InputPosition + 1 >= Input.Length)
                            throw new InvalidDataException($"Reached end of input while reading an escape sequence in a quoted string starting at position {TokenPosition}.");

                        InputPosition++;
                    }

                    InputPosition++;
                }

                throw new InvalidDataException($"Reached end of input while reading a quoted string starting at position {TokenPosition}.");

            }
            default:
            {
                TokenType = TextNbtTokenType.Value;
                InputPosition++;
                while (InputPosition < Input.Length
                       && !IsSymbol(Input.Span[InputPosition])
                       && !char.IsWhiteSpace(Input.Span[InputPosition])
                       && Input.Span[InputPosition] != '"'
                       && Input.Span[InputPosition] != '\'')
                    InputPosition++;

                TokenMemory = Input.Slice(TokenPosition, InputPosition - TokenPosition);
                return true;
            }
        }
    }
}