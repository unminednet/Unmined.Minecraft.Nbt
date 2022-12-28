using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Unmined.Minecraft.Nbt.Extensions;
using Unmined.Minecraft.Nbt.Parsers;
using Unmined.Minecraft.Nbt.Serializers;
using Unmined.Minecraft.Nbt.Tags;
using Xunit;
using Xunit.Abstractions;

namespace Unmined.Minecraft.Nbt.UnitTests.Examples;

public class ExamplesTest
{
    private readonly ITestOutputHelper _output;

    const string JavaEditionLevelDatFileName = "Data/java.level.dat";

    public ExamplesTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void SimpleExamples()
    {
        var nbt = new CompoundTag()
            .Add("First", 42)
            .Add("Second", "Hello World")
            .Add("Subtree 1", c => c
                .Add("PI", 3.14f)
                .Add("Numbers", new[] { 1, 2, 3, 4 }))
            .Add("Subtree 2", c => c
                .Add("List", new[] { "apple", "melon", "banana" }));

        Assert.Equal(3.14f, nbt.Find("Subtree 1/PI")?.GetAsFloat());
        Assert.Equal(3.14f, nbt.Find<FloatTag>("Subtree 1/PI")?.Value);
        Assert.Equal("banana", nbt.Find<ListTag>("Subtree 2/List")?[2].GetAsString());

        var data = NbtConvert.Serialize(nbt, BinaryNbtFormat.JavaEdition);
        var text = NbtConvert.Serialize(nbt, true);

        var nbtFromText = NbtConvert.Deserialize(text);
        var nbtFromBinary = NbtConvert.Deserialize(data, BinaryNbtFormat.JavaEdition);
    }

    [Fact]
    public void ReadSeedFromJavaEditionLevelDatExample()
    {
        var stream =
            new GZipStream(
                new FileStream(
                    JavaEditionLevelDatFileName,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite | FileShare.Delete),
                CompressionMode.Decompress);

        var nbt = NbtConvert.Deserialize(stream, BinaryNbtFormat.JavaEdition);

        var seed = nbt.Find("Data/WorldGenSettings/seed")?.GetAsString();
        _output.WriteLine("World seed: " + seed);
    }

    [Fact]
    public void ReadSeedFromJavaEditionLevelDatUsingParserExample()
    {
        var stream =
            new GZipStream(
                new FileStream(
                    JavaEditionLevelDatFileName,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite | FileShare.Delete),
                CompressionMode.Decompress);

        using var parser = new BinaryNbtStreamParser(stream, BinaryNbtFormat.JavaEdition);

        // use UTF8 byte arrays for tag name matching to prevent UTF8 => String conversions
        var nameData = Encoding.UTF8.GetBytes("Data");
        var nameWorldGenSettings = Encoding.UTF8.GetBytes("WorldGenSettings");
        var nameSeed = Encoding.UTF8.GetBytes("seed");

        // the parser is at the root tag
        // start reading it's children
        parser.BeginChildren();
        while (parser.FetchNextSibling())
        {
            // we are looking for the "Data" tag, skip all other tags
            if (!parser.NameEquals(nameData))
                continue;

            // "Data" tag found, start reading it's children
            parser.BeginChildren();
            while (parser.FetchNextSibling())
            {
                // we are looking for the "WorldGenSettings" tag, skip all other
                if (!parser.NameEquals(nameWorldGenSettings))
                    continue;

                // tag found, read it's children
                parser.BeginChildren();
                while (parser.FetchNextSibling())
                {
                    if (parser.NameEquals(nameSeed))
                    {
                        // "seed" tag found, print value and stop parsing
                        var seed = parser.GetAsString();
                        _output.WriteLine("World seed: " + seed);
                        return;
                    }
                }
            }
        }
    }


}