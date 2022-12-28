using System;
using System.Globalization;
using System.Linq;
using Unmined.Minecraft.Nbt.Extensions;
using Unmined.Minecraft.Nbt.Serializers;
using Unmined.Minecraft.Nbt.Tags;
using Xunit;

namespace Unmined.Minecraft.Nbt.UnitTests.Serializers;

public class TextDeserializerTest
{
    [Theory]
    [InlineData(0)]
    [InlineData(sbyte.MinValue)]
    [InlineData(sbyte.MaxValue)]
    public void DeserializeByte(sbyte value)
    {
        var tag = NbtConvert.Deserialize(value.ToString(CultureInfo.InvariantCulture) + "b");
        Assert.Equal(TagType.Byte, tag.Type);
        Assert.Equal(value, tag.GetAsSByte());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(short.MinValue)]
    [InlineData(short.MaxValue)]
    public void DeserializeShort(short value)
    {
        var tag = NbtConvert.Deserialize(value.ToString(CultureInfo.InvariantCulture) + "s");
        Assert.Equal(TagType.Short, tag.Type);
        Assert.Equal(value, tag.GetAsShort());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void DeserializeInt(int value)
    {
        var tag = NbtConvert.Deserialize(value.ToString(CultureInfo.InvariantCulture));
        Assert.Equal(TagType.Int, tag.Type);
        Assert.Equal(value, tag.GetAsInt());
    }


    [Theory]
    [InlineData(0)]
    [InlineData(long.MinValue)]
    [InlineData(long.MaxValue)]
    public void DeserializeLong(long value)
    {
        var tag = NbtConvert.Deserialize(value.ToString(CultureInfo.InvariantCulture) + "l");
        Assert.Equal(TagType.Long, tag.Type);
        Assert.Equal(value, tag.GetAsLong());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(float.MinValue)]
    [InlineData(float.MaxValue)]
    public void DeserializeFloat(float value)
    {
        var tag = NbtConvert.Deserialize(value.ToString(CultureInfo.InvariantCulture) + "f");
        Assert.Equal(TagType.Float, tag.Type);
        Assert.Equal(value, tag.GetAsFloat());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(double.MinValue)]
    [InlineData(double.MaxValue)]
    public void DeserializeDouble(double value)
    {
        var tag = NbtConvert.Deserialize(value.ToString(CultureInfo.InvariantCulture) + "d");
        Assert.Equal(TagType.Double, tag.Type);
        Assert.Equal(value, tag.GetAsDouble());
    }

    [Theory]
    [InlineData("''", "")]
    [InlineData("helloworld", "helloworld")]
    [InlineData("'hello world'", "hello world")]
    [InlineData("'hello \" world'", "hello \" world")]
    [InlineData("'hello \\' world'", "hello ' world")]
    [InlineData("\"hello world\"", "hello world")]
    [InlineData("\"hello \\\\ \\\" \n \r \t world\"", "hello \\ \" \n \r \t world")]
    public void DeserializeString(string input, string value)
    {
        var tag = NbtConvert.Deserialize(input);
        Assert.Equal(TagType.String, tag.Type);
        Assert.Equal(value, tag.GetAsString(), StringComparer.InvariantCulture);
    }

    [Theory]
    [InlineData("[B;]", new byte[] { })]
    [InlineData("[B;1b]", new byte[] { 1 })]
    [InlineData("[B;1b,2b]", new byte[] { 1, 2 })]
    [InlineData("[B;,1b,2b,]", new byte[] { 1, 2 })]
    public void DeserializeByteArray(string input, byte[] value)
    {
        var tag = NbtConvert.Deserialize(input);
        Assert.Equal(TagType.ByteArray, tag.Type);
        Assert.Equal(value, tag.GetAsByteArray());
    }

    [Theory]
    [InlineData("[I;]", new int[] { })]
    [InlineData("[I;1]", new[] { 1 })]
    [InlineData("[I;1,2]", new[] { 1, 2 })]
    [InlineData("[I;,1,2,]", new[] { 1, 2 })]
    public void DeserializeIntArray(string input, int[] value)
    {
        var tag = NbtConvert.Deserialize(input);
        Assert.Equal(TagType.IntArray, tag.Type);
        Assert.Equal(value, tag.GetAsIntArray());
    }

    [Theory]
    [InlineData("[L;]", new long[] { })]
    [InlineData("[L;1L]", new long[] { 1 })]
    [InlineData("[L;1L,2L]", new long[] { 1, 2 })]
    [InlineData("[L;,1L,2L,]", new long[] { 1, 2 })]
    public void DeserializeLongArray(string input, long[] value)
    {
        var tag = NbtConvert.Deserialize(input);
        Assert.Equal(TagType.LongArray, tag.Type);
        Assert.Equal(value, tag.GetAsLongArray());
    }

    [Theory]
    [InlineData("['a']", new[] { "a" })]
    [InlineData("['a', 'b']", new[] { "a", "b" })]
    public void DeserializeStringList(string input, string[] value)
    {
        var tag = NbtConvert.Deserialize(input);
        Assert.Equal(TagType.List, tag.Type);
        Assert.Equal(TagType.String, ((ListTag)tag).ItemType);
        Assert.Equal(value, ((ListTag)tag).Select(t => t.GetAsString()));
    }

    [Theory]
    [InlineData("{}", new string[] { })]
    [InlineData("{name:value}", new[] { "name", "value" })]
    [InlineData("{name:value,'number':42}", new[] { "name", "value", "number", "42" })]
    public void DeserializeCompound(string input, string[] value)
    {
        var tag = NbtConvert.Deserialize(input);
        Assert.Equal(TagType.Compound, tag.Type);
        Assert.Equal(
            ((CompoundTag)tag).SelectMany(v => new[] { v.Key, v.Value.GetAsString() }).ToArray(),
            value);
    }

    [Fact]
    public void DeserializeEmptyList()
    {
        var tag = NbtConvert.Deserialize("[]");
        Assert.Equal(TagType.List, tag.Type);
        Assert.Equal(TagType.End, ((ListTag)tag).ItemType);
    }
}