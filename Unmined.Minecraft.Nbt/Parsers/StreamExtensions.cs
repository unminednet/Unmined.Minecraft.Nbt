namespace Unmined.Minecraft.Nbt.Parsers;

public static class StreamExtensions
{
    private static readonly byte[] NullBuffer = new byte[4096];

    public static void Skip(this Stream stream, int numBytes)
    {
        if (stream.CanSeek)
            stream.Seek(numBytes, SeekOrigin.Current);
        else
            while (numBytes > 0)
            {
                var bytesToRead = Math.Min(numBytes, NullBuffer.Length);
                var bytesRead = stream.Read(NullBuffer, 0, bytesToRead);
                if (bytesRead < 0)
                    throw new EndOfStreamException();

                numBytes -= bytesRead;
            }
    }
}