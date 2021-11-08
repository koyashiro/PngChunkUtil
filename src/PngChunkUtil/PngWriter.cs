using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace KoyashiroKohaku.PngChunkUtil
{
    public static class PngWriter
    {
        public static byte[] WriteBytes(ReadOnlySpan<Chunk> chunks)
        {
            if (chunks.Length == 0)
            {
                throw new ArgumentException("invalid chunks");
            }

            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);

            binaryWriter.Write(Png.Signature);

            foreach (var chunk in chunks)
            {
                if (!chunk.IsValid())
                {
                    throw new ArgumentException("broken chunk");
                }

                binaryWriter.Write(chunk.Bytes);
            }

            return memoryStream.ToArray();
        }

        public static bool TryWriteBytes(ReadOnlySpan<Chunk> chunks, [MaybeNullWhen(false)] out byte[] output)
        {
            if (chunks.Length == 0)
            {
                output = default;
                return false;
            }

            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);

            binaryWriter.Write(Png.Signature);

            foreach (var chunk in chunks)
            {
                if (!chunk.IsValid())
                {
                    output = default;
                    return false;
                }

                binaryWriter.Write(chunk.Bytes);
            }

            output = memoryStream.ToArray();
            return true;
        }
    }
}
