using System.Globalization;
using Unmined.Minecraft.Nbt.Parsers;
using Unmined.Minecraft.Nbt.Tags;

namespace Unmined.Minecraft.Nbt.Serializers;

public static class TextNbtDeserializer
{
    public static Tag Deserialize(ITextNbtParser parser)
    {
        if (!parser.MoveNext())
            throw new InvalidDataException("Empty input");

        try
        {
            return ReadTag(parser);
        }
        catch (Exception e)
        {
            e.Data[nameof(parser.TokenPosition)] = parser.TokenPosition;
            throw;
        }
    }

    private static sbyte ParseByteValue(ITextNbtParser parser)
    {
        if (parser.Token[^1] is 'B' or 'b'
            && sbyte.TryParse(parser.Token[..^1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
            return v;

        throw new InvalidDataException(
            $"Error parsing byte value \"{parser.Token}\" at position {parser.TokenPosition}");
    }

    private static double ParseDoubleValue(ITextNbtParser parser)
    {
        if (parser.Token[^1] is 'D' or 'd'
            && double.TryParse(parser.Token[..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            return v;

        throw new InvalidDataException(
            $"Error parsing double value \"{parser.Token}\" at position {parser.TokenPosition}");
    }

    private static float ParseFloatValue(ITextNbtParser parser)
    {
        if (parser.Token[^1] is 'F' or 'f'
            && float.TryParse(parser.Token[..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            return v;

        throw new InvalidDataException(
            $"Error parsing float value \"{parser.Token}\" at position {parser.TokenPosition}");
    }

    private static int ParseIntValue(ITextNbtParser parser)
    {
        if (int.TryParse(parser.Token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
            return v;

        throw new InvalidDataException(
            $"Error parsing int value \"{parser.Token}\" at position {parser.TokenPosition}");
    }

    private static long ParseLongValue(ITextNbtParser parser)
    {
        if (parser.Token[^1] is 'L' or 'l'
            && long.TryParse(parser.Token[..^1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
            return v;

        throw new InvalidDataException(
            $"Error parsing long value \"{parser.Token}\" at position {parser.TokenPosition}");
    }


    private static string ParseQuotedStringValue(ITextNbtParser parser)
    {
        var token = parser.Token;

        var s = new char[token.Length];
        var len = 0;

        var quote = parser.Token[0];
        if (token[^1] != quote)
            throw new InvalidDataException(
                $"Missing ending quote from string \"{token}\" at position {parser.TokenPosition}");

        var original = token;
        token = token[1..^1];

        while (!token.IsEmpty)
        {
            if (token[0] == quote)
                throw new InvalidDataException(
                    $"Unescaped quote character in string \"{original}\" at position {parser.TokenPosition}");

            if (token[0] == '\\')
            {
                if (token.Length < 2)
                    throw new EndOfStreamException();

                switch (token[1])
                {
                    case '\\':
                        s[len++] = '\\';
                        break;
                    case 't':
                        s[len++] = '\t';
                        break;
                    case 'n':
                        s[len++] = '\n';
                        break;
                    case 'r':
                        s[len++] = '\r';
                        break;
                    default:
                    {
                        if (token[1] == quote)
                            s[len++] = quote;
                        else
                            throw new InvalidDataException(
                                $"Invalid escape sequence in string \"{original}\" at position {parser.TokenPosition}");
                        break;
                    }
                }

                token = token[2..];
                continue;
            }


            s[len++] = token[0];
            token = token[1..];
        }

        var result = new string(s[..len]);
        return result;
    }

    private static short ParseShortValue(ITextNbtParser parser)
    {
        if (parser.Token[^1] is 'S' or 's'
            && short.TryParse(parser.Token[..^1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
            return v;

        throw new InvalidDataException(
            $"Error parsing short value \"{parser.Token}\" at position {parser.TokenPosition}");
    }

    private static Tag ReadArrayTag(ITextNbtParser parser)
    {
        return parser.Token[1] switch
        {
            'B' => ReadByteArrayTag(parser),
            'I' => ReadIntArrayTag(parser),
            'L' => ReadLongArrayTag(parser),
            _ => throw new InvalidDataException(
                $"Invalid array type \"{parser.Token[1]}\" at position {parser.TokenPosition}")
        };
    }

    private static Tag ReadByteArrayTag(ITextNbtParser parser)
    {
        var startPosition = parser.TokenPosition;

        var items = new List<byte>();
        while (parser.MoveNext())
            switch (parser.TokenType)
            {
                case TextNbtTokenType.EndList:
                    return new ByteArrayTag(items.ToArray());
                case TextNbtTokenType.Separator:
                    continue;
                case TextNbtTokenType.Value:
                    items.Add(unchecked((byte)ParseByteValue(parser)));
                    continue;
                default:
                    throw new InvalidDataException(
                        $"Invalid token \"{parser.Token}\" found while reading a byte array at position {parser.TokenPosition}");
            }

        throw new InvalidDataException(
            $"End of input reached while reading a byte array starting at position {startPosition}");
    }

    private static Tag ReadCompoundTag(ITextNbtParser parser)
    {
        var startPosition = parser.TokenPosition;

        var items = new Dictionary<string, Tag>();
        while (parser.MoveNext())
            switch (parser.TokenType)
            {
                case TextNbtTokenType.EndCompound:
                    return new CompoundTag(items);
                case TextNbtTokenType.Separator:
                    continue;
                case TextNbtTokenType.Value:
                {
                    var name = parser.Token[0] is '"' or '\''
                        ? ParseQuotedStringValue(parser)
                        : new string(parser.Token);

                    if (!parser.MoveNext())
                        throw new InvalidDataException(
                            $"End of input reached while reading a compound tag starting at position {startPosition}");

                    if (parser.TokenType != TextNbtTokenType.NameValueSeparator)
                        throw new InvalidDataException(
                            $"Invalid token \"{parser.Token}\" found while reading a compound tag at position {parser.TokenPosition}");

                    if (!parser.MoveNext())
                        throw new InvalidDataException(
                            $"End of input reached while reading a compound tag starting at position {startPosition}");

                    var value = ReadValueTag(parser);

                    items.Add(name, value);

                    continue;
                }
                default:
                    throw new InvalidDataException(
                        $"Invalid token \"{parser.Token}\" found while reading a compound tag at position {parser.TokenPosition}");
            }

        throw new InvalidDataException(
            $"End of input reached while reading a compound tag starting at position {startPosition}");
    }

    private static Tag ReadIntArrayTag(ITextNbtParser parser)
    {
        var startPosition = parser.TokenPosition;

        var items = new List<int>();
        while (parser.MoveNext())
            switch (parser.TokenType)
            {
                case TextNbtTokenType.EndList:
                    return new IntArrayTag(items.ToArray());
                case TextNbtTokenType.Separator:
                    continue;
                case TextNbtTokenType.Value:
                    items.Add(ParseIntValue(parser));
                    continue;
                default:
                    throw new InvalidDataException(
                        $"Invalid token \"{parser.Token}\" found while reading an int array at position {parser.TokenPosition}");
            }

        throw new InvalidDataException(
            $"End of input reached while reading an int array starting at position {startPosition}");
    }

    private static Tag ReadListTag(ITextNbtParser parser)
    {
        var startPosition = parser.TokenPosition;

        var items = new List<Tag>();
        var type = TagType.End;

        while (parser.MoveNext())
        {
            switch (parser.TokenType)
            {
                case TextNbtTokenType.EndList:
                    return new ListTag(type, items);
                case TextNbtTokenType.Separator:
                    continue;
            }

            var pos = parser.TokenPosition;
            var tag = ReadTag(parser);

            if (type != TagType.End && tag.Type != type)
                throw new InvalidDataException(
                    $"Unexpected tag type \"{tag.Type}\" in list of \"{type}\" at position {pos}");

            type = tag.Type;
            items.Add(tag);
        }

        throw new InvalidDataException(
            $"End of input reached while reading a list tag starting at position {startPosition}");
    }

    private static Tag ReadLongArrayTag(ITextNbtParser parser)
    {
        var startPosition = parser.TokenPosition;

        var items = new List<long>();
        while (parser.MoveNext())
            switch (parser.TokenType)
            {
                case TextNbtTokenType.EndList:
                    return new LongArrayTag(items.ToArray());
                case TextNbtTokenType.Separator:
                    continue;
                case TextNbtTokenType.Value:
                    items.Add(ParseLongValue(parser));
                    continue;
                default:
                    throw new InvalidDataException(
                        $"Invalid token \"{parser.Token}\" found while reading a long array at position {parser.TokenPosition}");
            }

        throw new InvalidDataException(
            $"End of input reached while reading a long array starting at position {startPosition}");
    }

    private static Tag ReadNumberTag(ITextNbtParser parser)
    {
        return parser.Token[^1] switch
        {
            'b' or 'B' => new ByteTag(ParseByteValue(parser)),
            's' or 'S' => new ShortTag(ParseShortValue(parser)),
            'l' or 'L' => new LongTag(ParseLongValue(parser)),
            'd' or 'D' => new DoubleTag(ParseDoubleValue(parser)),
            'f' or 'F' => new FloatTag(ParseFloatValue(parser)),
            >= '0' and <= '9' => new IntTag(ParseIntValue(parser)),
            _ => throw new InvalidDataException(
                $"Invalid number type indicator found while reading a number \"{parser.Token}\" at position {parser.TokenPosition}")
        };
    }

    private static Tag ReadTag(ITextNbtParser parser)
    {
        return parser.TokenType switch
        {
            TextNbtTokenType.BeginCompound => ReadCompoundTag(parser),
            TextNbtTokenType.BeginList => ReadListTag(parser),
            TextNbtTokenType.BeginArray => ReadArrayTag(parser),
            TextNbtTokenType.Value => ReadValueTag(parser),
            _ => throw new InvalidDataException(
                $"Invalid token \"{parser.Token}\" found while reading a tag at position {parser.TokenPosition}")
        };
    }

    private static Tag ReadValueTag(ITextNbtParser parser)
    {
        // boolean

        if (parser.Token.SequenceEqual("false")) return new ByteTag(0);
        if (parser.Token.SequenceEqual("true")) return new ByteTag(1);

        // string

        if (parser.Token[0] is '"' or '\'')
            return new StringTag(ParseQuotedStringValue(parser));

        if (char.IsLetter(parser.Token[0]))
            return new StringTag(new string(parser.Token));

        // number

        if (parser.Token[0] >= '0' && parser.Token[0] <= '9' || parser.Token[0] is '+' or '-' or '.')
            return ReadNumberTag(parser);

        throw new InvalidDataException(
            $"Invalid value \"{parser.Token}\" found while reading a tag at position {parser.TokenPosition}");
    }
}