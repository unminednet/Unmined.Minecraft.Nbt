namespace Unmined.Minecraft.Nbt;

public enum TagType
{
    /// <summary>
    ///     Used to mark the end of compound tags
    /// </summary>
    End = 0,

    /// <summary>
    ///     8-bit signed integer
    /// </summary>
    Byte = 1,

    /// <summary>
    ///     16-bit signed integer
    /// </summary>
    Short = 2,

    /// <summary>
    ///     32-bit signed integer
    /// </summary>
    Int = 3,

    /// <summary>
    ///     64-bit signed integer
    /// </summary>
    Long = 4,

    /// <summary>
    ///     32-bit float (single precision)
    /// </summary>
    Float = 5,

    /// <summary>
    ///     64-bit float (double precision)
    /// </summary>
    Double = 6,

    /// <summary>
    ///     Array of bytes
    /// </summary>
    ByteArray = 7,

    /// <summary>
    ///     UTF8 string
    /// </summary>
    String = 8,

    /// <summary>
    ///     Array of nameless tags of the same type
    /// </summary>
    List = 9,

    /// <summary>
    ///     Array of named tags of various types
    /// </summary>
    Compound = 10,

    /// <summary>
    ///     Array of signed 32-bit integers
    /// </summary>
    IntArray = 11,

    /// <summary>
    ///     Array of signed 64-bit integers
    /// </summary>
    LongArray = 12
}