namespace Unmined.Minecraft.Nbt.Writers;

public class TextNbtWriter : IDisposable
{
    private const int MaxIndent = 120;

    private static readonly string IndentChars = new(' ', MaxIndent);
    private readonly bool _leaveOpen;
    private readonly bool _humanFriendly;

    private int _indent;

    public TextNbtWriter(TextWriter writer, bool leaveOpen = false, bool humanFriendly = false)
    {
        _leaveOpen = leaveOpen;
        _humanFriendly = humanFriendly;
        Writer = writer;
    }

    public TextWriter Writer { get; }

    public void BeginCompound()
    {
        if (_humanFriendly)
        {
            Writer.WriteLine('{');
            _indent++;
        }
        else
        {
            Writer.Write('{');
        }
    }

    public void BeginCompoundItem()
    {
        if (_humanFriendly) WriteIndent();
    }

    public void BeginList()
    {
        if (_humanFriendly)
        {
            Writer.WriteLine('[');
            _indent++;
        }
        else
        {
            Writer.Write('[');
        }
    }

    public void BeginListItem()
    {
        if (_humanFriendly) WriteIndent();
    }

    public void BeginRoot(ReadOnlySpan<char> name)
    {
        if (!name.IsEmpty)
            throw new InvalidOperationException("Root tag name is not supported by SNBT format.");

        if (_humanFriendly)
        {
            Writer.WriteLine("{");
            _indent++;
        }
        else
        {
            Writer.Write('{');
        }
    }

    public void EndCompound()
    {
        if (_humanFriendly)
        {
            Writer.WriteLine();
            _indent--;
            WriteIndent();
            Writer.Write("}");
        }
        else
        {
            Writer.Write('}');
        }
    }

    public void EndCompoundItem()
    {
    }

    public void EndList()
    {
        if (_humanFriendly)
        {
            Writer.WriteLine();
            _indent--;
            WriteIndent();
            Writer.Write("]");
        }
        else
        {
            Writer.Write(']');
        }
    }

    public void EndListItem()
    {
    }

    public void EndRoot()
    {
        EndCompound();
    }

    public void WriteBool(bool value)
    {
        Writer.Write(value ? "true" : "false");
    }

    public void WriteByteArray(ReadOnlySpan<byte> value)
    {
        Writer.Write("[B;");
        var maxValuesPerLine = 16;
        var multiline = _humanFriendly && value.Length > maxValuesPerLine;
        if (multiline)
        {
            Writer.WriteLine();
            _indent++;
        }

        var isFirstWritten = false;
        var c = 0;
        foreach (var v in value)
        {
            if (isFirstWritten)
            {
                if (_humanFriendly) Writer.Write(", ");
                else Writer.Write(',');
                if (multiline && c >= maxValuesPerLine)
                {
                    Writer.WriteLine();
                    WriteIndent();
                    c = 0;
                }
            }
            else
            {
                if (multiline) WriteIndent();
                isFirstWritten = true;
            }

            Writer.Write(v);
            Writer.Write('B');
            c++;
        }

        if (multiline)
        {
            Writer.WriteLine();
            _indent--;
            WriteIndent();
        }

        Writer.Write(']');
    }

    public void WriteCompoundSeparator()
    {
        if (_humanFriendly)
            Writer.WriteLine(',');
        else
            Writer.Write(',');
    }

    public void WriteDouble(double value)
    {
        Writer.Write(value);
        Writer.Write('D');
    }

    public void WriteFloat(float value)
    {
        Writer.Write(value);
        Writer.Write('F');
    }

    public void WriteInt(int value)
    {
        Writer.Write(value);
    }

    public void WriteIntArray(ReadOnlySpan<int> value)
    {
        Writer.Write("[I;");
        var maxValuesPerLine = 8;
        var multiline = _humanFriendly && value.Length > maxValuesPerLine;
        if (multiline)
        {
            Writer.WriteLine();
            _indent++;
        }

        var isFirstWritten = false;
        var c = 0;
        foreach (var v in value)
        {
            if (isFirstWritten)
            {
                if (_humanFriendly) Writer.Write(", ");
                else Writer.Write(',');
                if (multiline && c >= maxValuesPerLine)
                {
                    Writer.WriteLine();
                    WriteIndent();
                    c = 0;
                }
            }
            else
            {
                if (multiline) WriteIndent();
                isFirstWritten = true;
            }

            Writer.Write(v);
            c++;
        }

        if (multiline)
        {
            Writer.WriteLine();
            _indent--;
            WriteIndent();
        }

        Writer.Write(']');
    }

    public void WriteListSeparator()
    {
        if (_humanFriendly)
            Writer.WriteLine(',');
        else
            Writer.Write(',');
    }

    public void WriteLong(long value)
    {
        Writer.Write(value);
        Writer.Write('l');
    }

    public void WriteLongArray(ReadOnlySpan<long> value)
    {
        Writer.Write("[L;");
        var maxValuesPerLine = 4;
        var multiline = _humanFriendly && value.Length > maxValuesPerLine;
        if (multiline)
        {
            Writer.WriteLine();
            _indent++;
        }


        var isFirstWritten = false;
        var c = 0;
        foreach (var v in value)
        {
            if (isFirstWritten)
            {
                if (_humanFriendly) Writer.Write(", ");
                else Writer.Write(',');
                if (multiline && c >= maxValuesPerLine)
                {
                    Writer.WriteLine();
                    WriteIndent();
                    c = 0;
                }
            }
            else
            {
                if (multiline) WriteIndent();
                isFirstWritten = true;
            }

            Writer.Write(v);
            Writer.Write('L');
            c++;
        }

        if (multiline)
        {
            Writer.WriteLine();
            _indent--;
            WriteIndent();
        }

        Writer.Write(']');
    }

    public void WriteName(ReadOnlySpan<char> name)
    {
        if (NeedsQuotes(name))
            WriteString(name, false);
        else
            Writer.Write(name);

        if (_humanFriendly)
            Writer.Write(": ");
        else
            Writer.Write(':');
    }

    public void WriteSByte(sbyte value)
    {
        Writer.Write(value);
        Writer.Write('B');
    }

    public void WriteShort(short value)
    {
        Writer.Write(value);
        Writer.Write('S');
    }

    public void WriteString(ReadOnlySpan<char> value)
    {
        WriteString(value, true);
    }

    public void Dispose()
    {
        if (!_leaveOpen)
            Writer.Dispose();
    }

    private bool NeedsQuotes(ReadOnlySpan<char> value)
    {
        foreach (var c in value)
            if (!char.IsLetterOrDigit(c) && c != '_')
                return true;

        return false;
    }

    private void WriteIndent()
    {
        if (_indent <= 0)
            return;

        var i = _indent * 4;
        if (i > IndentChars.Length)
            i = IndentChars.Length;

        Writer.Write(IndentChars.AsSpan(0, i));
    }

    private void WriteString(ReadOnlySpan<char> value, bool allowSingleQuote)
    {
        var quote = allowSingleQuote && value.IndexOf('\"') >= 0 ? '\'' : '"';
        var toEscape = quote == '"'
            ? "\"\\\n\r\t"
            : "'\\\n\r\t";

        Writer.Write(quote);

        while (!value.IsEmpty)
        {
            var i = value.IndexOfAny(toEscape);
            if (i < 0)
            {
                Writer.Write(value);
                break;
            }

            Writer.Write(value[..i]);
            Writer.Write(value[i] switch
            {
                '"' => "\\\"",
                '\\' => "\\\\",
                '\'' => "\\'",
                '\n' => "\\n",
                '\r' => "\\r",
                '\t' => "\\t",
                _ => value[i].ToString()
            });
            value = value[(i + 1)..];
        }

        Writer.Write(quote);
    }
}