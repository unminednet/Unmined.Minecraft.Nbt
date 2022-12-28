# uNmINeD NBT Library

![Build status](https://github.com/unminednet/unmined.minecraft.nbt/actions/workflows/publish.yml/badge.svg)


## Introduction

uNmINeD NBT Library is a C# class library for working with the Named Binary Tag data format used by Minecraft.

It provides:

* Tag classes to represent a data tree
* Parsers for reading NBT binary streams and memory buffers
* Serializers/deserializers for text and binary formats

Features:

* High performance and poolable binary parsers using Span&lt;T&gt;
* LINQ compatible compound and list tags
* Both Java Edition and Bedrock Edition formats are supported

The library was originally developed for uNmINeD, a Minecraft world viewer and mapper, and it's designed for reading NBT data at high speed with minimal memory allocation stress.

## Quickstart

Creating an NBT structure using fluent-style syntax:

```csharp
var nbt = new CompoundTag()
    .Add("First", 42)
    .Add("Second", "Hello World")
    .Add("Subtree 1", c => c
        .Add("PI", 3.14f)
        .Add("Numbers", new[] { 1, 2, 3, 4 }))
    .Add("Subtree 2", c => c
        .Add("List", new[] { "apple", "melon", "banana" }));

```

Querying:

````csharp
nbt.Find<FloatTag>("Subtree 1/PI")?.Value; // 3.14
nbt.Find("Subtree 1/PI")?.GetAsFloat()); // 3.14
nbt.Find<ListTag>("Subtree 2/List")?[2].GetAsString(); // "banana"

````

Serializing/deserializing to binary format:

````csharp
byte[] data = NbtConvert.Serialize(nbt, BinaryNbtFormat.JavaEdition);
var nbtFromBinary = NbtConvert.Deserialize(data, BinaryNbtFormat.JavaEdition);
````

Serializing/deserializing to SNBT text format:

```csharp
var text = NbtConvert.Serialize(nbt, true);
Console.WriteLine(text);
var nbtFromText = NbtConvert.Deserialize(text);

````

SNBT output using human friendly formatting:

````console
{
    First: 42,
    Second: "Hello World",
    "Subtree 1": {
        PI: 3.14f,
        Numbers: [I;1,2,3,4]
    },
    "Subtree 2": {
        List: [
            "apple",
            "melon",
            "banana"
        ]
    }
}
````

## Usage examples

### Get world seed from level.dat (Java Edition):

```csharp
var stream =
    new GZipStream(
        new FileStream(
            @"c:\path\to\my\level.dat", 
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete),
            CompressionMode.Decompress);

var nbt = NbtConvert.Deserialize(stream, BinaryNbtFormat.JavaEdition);

var seed = nbt.Find("Data/WorldGenSettings/seed")?.GetAsString();
Console.WriteLine("World seed: " + seed);
```

### Get world seed from level.dat using the parser:

Values can be read from an NBT stream using the low level parser, which provides much better performance than loading the entire NBT into an object tree.

```csharp
var stream =
    new GZipStream(
        new FileStream(
            @"c:\path\to\my\level.dat", 
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete),
            CompressionMode.Decompress);

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
                Console.WriteLine("World seed: " + seed);
                return;
            }
        }
    }
}

```

## Credits

The NBT format was designed by Markus Persson (Notch), the original creator of Minecraft.

The uNmINeD NBT Library was created by Balázs Farkas (megasys), the author of uNmINeD.

## License

uNmINeD NBT Library is released under the MIT license.

## Contributing

Pull requests are welcome. Please always open an issue before starting to work on a pull request.

You can support me on [Patreon](https://www.patreon.com/megasys).

## Legal notes

The uNmINeD NBT Library is not an official Minecraft product, and is not approved by or associated with Mojang.
