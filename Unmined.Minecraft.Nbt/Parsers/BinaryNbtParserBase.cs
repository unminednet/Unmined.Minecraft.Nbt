using System.Buffers;
using System.Buffers.Binary;
using System.Globalization;
using System.Text;
using CommunityToolkit.HighPerformance;
using Unmined.Allocation;

namespace Unmined.Minecraft.Nbt.Parsers;

public abstract class BinaryNbtParserBase : IBufferedBinaryNbtParser
{
    public static int StateStackDepth = 64;

    private ParserState[] _stateStack;
    private int _stateStackPointer;
    private ParserState _state;

    protected BinaryNbtParserBase(BinaryNbtFormat format) : this()
    {
        Format = format;
    }

    private BinaryNbtParserBase()
    {
        _stateStack = Allocator<ParserState>.Shared.Rent(StateStackDepth);
    }

    public abstract ReadOnlyMemory<byte> NameMemory { get; }
    public abstract ReadOnlyMemory<byte> DataMemory { get; }

    public BinaryNbtFormat Format { get; private set; }
    public int InputPosition { get; private set; }
    public int NamePosition => _state.NamePosition;
    public int NameLength => _state.NameLength;
    public int DataPosition => _state.DataPosition;
    public int DataLength => _state.DataLength;
    public TagType Type => _state.Type;
    public string Name => Encoding.UTF8.GetString(NameMemory.Span);
    public TagType ListItemType => _state.ListItemType;
    public int ListItemCount => _state.ListItemCount;

    public int ArrayLength => _state.ArrayLength;

    private static void CheckBufferSize(int actual, int expected)
    {
        if (actual != expected)
            throw new InvalidOperationException(
                $"Buffer size mismatch (expected: {expected}, actual: {actual})");
    }

    public void BeginRoot()
    {
        _state.Type = ReadTagType();
        if (_state.Type != TagType.Compound)
            throw new InvalidDataException($"Root tag type \"{_state.Type}\" is not Compound");

        ReadTagName();
    }

    public void GetData(Span<byte> buffer)
    {
        DataMemory.Span.CopyTo(buffer);
    }

    public bool NameEquals(ReadOnlySpan<byte> tagNameUtf8)
    {
        return NameMemory.Span.SequenceEqual(tagNameUtf8);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    public void BeginChildren()
    {
        switch (_state.Type)
        {
            case TagType.Compound:
                _state.AreItemsRead = true;
                Push(_state);

                _state.IsReadingCompoundItems = true;
                _state.IsReadingListItems = false;
                _state.ParsingListIndex = -1;
                _state.ParsingListCount = -1;
                _state.ParsingListItemType = TagType.End;
                _state.NameLength = -1;
                _state.NamePosition = -1;
                _state.DataLength = -1;
                _state.DataPosition = -1;
                break;
            case TagType.List:
                _state.AreItemsRead = true;
                Push(_state);

                _state.ParsingListItemType = _state.ListItemType;
                _state.ParsingListCount = _state.ListItemCount;
                _state.IsReadingCompoundItems = false;
                _state.IsReadingListItems = true;
                _state.ParsingListIndex = 0;
                _state.Type = _state.ParsingListItemType;
                _state.NameLength = -1;
                _state.NamePosition = -1;
                _state.DataLength = -1;
                _state.DataPosition = -1;
                break;
            default:
                throw new InvalidOperationException($"Cannot use BeginChildren() on tag type {_state.Type}");
        }
    }

    public void ExpectTagType(TagType tagType)
    {
        if (Type != tagType)
            throw new InvalidDataException(
                $"Unexpected tag type {Type} (expected {tagType})");
    }

    public bool FetchNextSibling()
    {
        if (!_state.AreItemsRead) SkipItems();
        _state.AreItemsRead = false;

        if (_state.IsReadingListItems)
        {
            _state.ParsingListIndex++;
            if (_state.ParsingListIndex > _state.ParsingListCount)
            {
                _state = Pop();
                return false;
            }

            LoadData();
            return true;
        }

        if (_state.IsReadingCompoundItems)
        {
            _state.Type = ReadTagType();
            if (_state.Type == TagType.End)
            {
                _state = Pop();
                return false;
            }

            if (_state.IsSkippingItems)
                SkipTagName();
            else
                ReadTagName();

            LoadData();
            return true;
        }

        throw new InvalidOperationException("FetchNextSibling() can only be called when reading compound or list items");
    }

    public string GetAsString()
    {
        switch (Type)
        {
            case TagType.Byte: return GetSByte().ToString(CultureInfo.InvariantCulture);
            case TagType.Short: return GetShort().ToString(CultureInfo.InvariantCulture);
            case TagType.Int: return GetInt().ToString(CultureInfo.InvariantCulture);
            case TagType.Long: return GetLong().ToString(CultureInfo.InvariantCulture);
            case TagType.Float: return GetFloat().ToString(CultureInfo.InvariantCulture);
            case TagType.Double: return GetDouble().ToString(CultureInfo.InvariantCulture);
            case TagType.String: return GetString();
            default:
                throw new InvalidOperationException($"Cannot use CurrentDataAsString() on tag type {Type}");
        }
    }

    public void GetByteArray(Span<byte> buffer)
    {
        ExpectTagType(TagType.ByteArray);
        CheckBufferSize(buffer.Length, ArrayLength);

        DataMemory.Span.CopyTo(buffer);
    }

    public double GetDouble()
    {
        ExpectTagType(TagType.Double);
        return Format switch
        {
            BinaryNbtFormat.JavaEdition => BinaryPrimitives.ReadDoubleBigEndian(DataMemory.Span),
            BinaryNbtFormat.BedrockEdition => BinaryPrimitives.ReadDoubleLittleEndian(DataMemory.Span),
            _ => throw new InvalidOperationException("Unknown format")
        };
    }

    public float GetFloat()
    {
        ExpectTagType(TagType.Float);
        return Format switch
        {
            BinaryNbtFormat.JavaEdition => BinaryPrimitives.ReadSingleBigEndian(DataMemory.Span),
            BinaryNbtFormat.BedrockEdition => BinaryPrimitives.ReadSingleLittleEndian(DataMemory.Span),
            _ => throw new InvalidOperationException("Unknown format")
        };
    }


    public int GetInt()
    {
        ExpectTagType(TagType.Int);
        return Format switch
        {
            BinaryNbtFormat.JavaEdition => BinaryPrimitives.ReadInt32BigEndian(DataMemory.Span),
            BinaryNbtFormat.BedrockEdition => BinaryPrimitives.ReadInt32LittleEndian(DataMemory.Span),
            _ => throw new InvalidOperationException("Unknown format")
        };
    }

    public void GetIntArray(Span<int> buffer)
    {
        ExpectTagType(TagType.IntArray);
        CheckBufferSize(buffer.Length, ArrayLength);

        DataMemory.Span.CopyTo(buffer.AsBytes());

        switch (Format)
        {
            case BinaryNbtFormat.JavaEdition:
                for (var i = 0; i < buffer.Length; i++)
                    buffer[i] = BinaryPrimitives.ReverseEndianness(buffer[i]);
                break;
            case BinaryNbtFormat.BedrockEdition:
                break;
            default:
                throw new InvalidOperationException("Unknown format");
        }
    }

    public long GetLong()
    {
        ExpectTagType(TagType.Long);
        return Format switch
        {
            BinaryNbtFormat.JavaEdition => BinaryPrimitives.ReadInt64BigEndian(DataMemory.Span),
            BinaryNbtFormat.BedrockEdition => BinaryPrimitives.ReadInt64LittleEndian(DataMemory.Span),
            _ => throw new InvalidOperationException("Unknown format")
        };
    }

    public void GetLongArray(Span<long> buffer)
    {
        ExpectTagType(TagType.LongArray);
        CheckBufferSize(buffer.Length, ArrayLength);

        DataMemory.Span.CopyTo(buffer.AsBytes());

        switch (Format)
        {
            case BinaryNbtFormat.JavaEdition:
                for (var i = 0; i < buffer.Length; i++)
                    buffer[i] = BinaryPrimitives.ReverseEndianness(buffer[i]);
                break;
            case BinaryNbtFormat.BedrockEdition:
                break;
            default:
                throw new InvalidOperationException("Unknown format");
        }
    }

    public sbyte GetSByte()
    {
        ExpectTagType(TagType.Byte);
        return (sbyte)DataMemory.Span[0];
    }

    public short GetShort()
    {
        ExpectTagType(TagType.Short);
        return Format switch
        {
            BinaryNbtFormat.JavaEdition => BinaryPrimitives.ReadInt16BigEndian(DataMemory.Span),
            BinaryNbtFormat.BedrockEdition => BinaryPrimitives.ReadInt16LittleEndian(DataMemory.Span),
            _ => throw new InvalidOperationException("Unknown format")
        };
    }

    public string GetString()
    {
        ExpectTagType(TagType.String);
        return Encoding.UTF8.GetString(DataMemory.Span);
    }

    protected abstract void InternalLoadData();

    protected abstract byte InternalReadByte();
    protected abstract int InternalReadInt();
    protected abstract short InternalReadShort();
    protected abstract void InternalReadTagName();
    protected abstract void InternalSkip(int len);

    protected virtual void Close()
    {
        _stateStackPointer = 0;
        _state = new ParserState();
        InputPosition = 0;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing) Allocator<ParserState>.Shared.Return(_stateStack);
    }

    protected virtual void Reset(BinaryNbtFormat format)
    {
        Format = format;
        _stateStackPointer = 0;
        _state = new ParserState();
        InputPosition = 0;
    }

    protected int ReadStringLength()
    {
        var length = Format switch
        {
            BinaryNbtFormat.JavaEdition => BinaryPrimitives.ReverseEndianness(InternalReadShort()),
            BinaryNbtFormat.BedrockEdition => InternalReadShort(),
            _ => throw new InvalidOperationException("Unknown format")
        };
        InputPosition += sizeof(short);
        return length;
    }

    private void LoadData()
    {
        _state.ArrayLength = _state.Type switch
        {
            TagType.ByteArray => ReadArrayLength(),
            TagType.IntArray => ReadArrayLength(),
            TagType.LongArray => ReadArrayLength(),
            _ => 0
        };

        _state.DataLength = _state.Type switch
        {
            TagType.Byte => sizeof(byte),
            TagType.Double => sizeof(double),
            TagType.Float => sizeof(float),
            TagType.Int => sizeof(int),
            TagType.ByteArray => _state.ArrayLength,
            TagType.IntArray => _state.ArrayLength * sizeof(int),
            TagType.LongArray => _state.ArrayLength * sizeof(long),
            TagType.Long => sizeof(long),
            TagType.Short => sizeof(short),
            TagType.String => ReadStringLength(),
            TagType.List => -1,
            TagType.Compound => -1,
            _ => throw new NotSupportedException($"Unknonwn tag type {_state.Type}")
        };


        if (_state.Type == TagType.List)
        {
            _state.ListItemType = ReadTagType();
            _state.ListItemCount = ReadArrayLength();
        }

        _state.DataPosition = _state.DataLength < 0 ? -1 : InputPosition;

        if (_state.DataLength > 0)
        {
            // todo: lazy data loading for stream parser
            InternalLoadData();
            InputPosition += _state.DataLength;
        }
    }

    private ParserState Pop()
    {
        if (_stateStackPointer == 0)
            throw new InvalidOperationException("Stack is empty");

        return _stateStack[--_stateStackPointer];
    }

    private void Push(ParserState state)
    {
        if (_stateStackPointer >= _stateStack.Length)
        {
            var newStack = Allocator<ParserState>.Shared.Rent(_stateStack.Length + 64);
            _stateStack.CopyTo(newStack.AsSpan());
            Allocator<ParserState>.Shared.Return(_stateStack);
            _stateStack = newStack;
        }

        _stateStack[_stateStackPointer++] = state;
    }

    private int ReadArrayLength()
    {
        var count = Format switch
        {
            BinaryNbtFormat.JavaEdition => BinaryPrimitives.ReverseEndianness(InternalReadInt()),
            BinaryNbtFormat.BedrockEdition => InternalReadInt(),
            _ => throw new InvalidOperationException("Unknown format")
        };
        InputPosition += sizeof(int);
        return count;
    }

    private void ReadTagName()
    {
        var len = ReadStringLength();
        _state.NamePosition = InputPosition;
        _state.NameLength = len;
        InternalReadTagName();
        InputPosition += len;
    }

    private TagType ReadTagType()
    {
        var b = InternalReadByte();
        if (!TagTypeValidator.IsValid(b))
            throw new InvalidDataException($"Unknown tag type ({b})");

        InputPosition++;

        return (TagType)b;
    }

    private void SkipItems()
    {
        if (_state.Type is not (TagType.Compound or TagType.List))
            return;

        _state.IsSkippingItems = true;
        try
        {
            BeginChildren();
            while (FetchNextSibling())
            {
            }
        }
        finally
        {
            _state.IsSkippingItems = false;
        }
    }

    private void SkipTagName()
    {
        var len = ReadStringLength();
        InternalSkip(len);
        InputPosition += len;
    }

    private struct ParserState
    {
        public ParserState()
        {
        }

        // ReSharper disable RedundantDefaultMemberInitializer
        public int DataLength = -1;
        public int DataPosition = -1;
        public int NameLength = -1;
        public int NamePosition = -1;
        public bool IsReadingCompoundItems = false;
        public bool IsReadingListItems = false;
        public bool AreItemsRead = false;
        public bool IsSkippingItems = false;
        public TagType ListItemType = TagType.End;
        public int ParsingListCount = -1;
        public int ParsingListIndex = -1;
        public TagType ParsingListItemType = TagType.End;
        public TagType Type = TagType.Compound;
        public int ListItemCount = -1;

        public int ArrayLength = -1;
        // ReSharper restore RedundantDefaultMemberInitializer
    }
}