using KoyashiroKohaku.PngChunkUtil.Properties;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace KoyashiroKohaku.PngChunkUtil
{
    public static class Png
    {
        private static ReadOnlySpan<byte> Signature => new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        private static bool IsValidSignature(byte[] source)
        {
            if (source is null)
            {
                return false;
            }

            return IsValidSignature(source.AsSpan());
        }

        private static bool IsValidSignature(ReadOnlySpan<byte> source)
        {
            if (source.Length < 8)
            {
                return false;
            }

            return source[0..8].SequenceEqual(Signature);
        }

        private static IEnumerable<Chunk> InternalSplitIntoChunks(byte[] image)
        {
            var index = 8;

            while (index < image.Length)
            {
                if (index + 8 > image.Length)
                {
                    throw new ArgumentException(Resources.ChunkReader_SplitChunk_InvalidChunk, nameof(image));
                }

                var length = BinaryPrimitives.ReadInt32BigEndian(image[index..(index + 4)]) + 12;

                if (index + length > image.Length)
                {
                    throw new ArgumentException(Resources.ChunkReader_SplitChunk_InvalidChunk, nameof(image));
                }

                yield return new Chunk(image, index..(index + length));
                index += length;
            }
        }

        private static bool InternalTrySplitIntoChunks(byte[] image, [MaybeNullWhen(false)] out Chunk[] chunks)
        {
            var buffer = image.AsSpan();
            var index = 8;

            var list = new List<Chunk>();

            while (index < buffer.Length)
            {
                if (index + 8 > buffer.Length)
                {
                    chunks = default;
                    return false;
                }

                var length = BinaryPrimitives.ReadInt32BigEndian(buffer[index..(index + 4)]) + 12;

                if (index + length > buffer.Length)
                {
                    chunks = default;
                    return false;
                }

                if (!Chunk.TryCreate(image, index..(index + length), out var chunk))
                {
                    chunks = default;
                    return false;
                }

                list.Add(chunk);
                index += length;
            }

            chunks = list.ToArray();
            return true;
        }

        public static IEnumerable<Chunk> SplitIntoChunks(byte[] image)
        {
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (!IsValidSignature(image))
            {
                throw new ArgumentException(Resources.ChunkReader_SplitChunk_InvalidImage, nameof(image));
            }

            return InternalSplitIntoChunks(image);
        }

        public static bool TrySplitIntoChunks(byte[] image, [MaybeNullWhen(false)] out Chunk[] chunks)
        {
            if (!IsValidSignature(image))
            {
                chunks = default;
                return false;
            }

            return InternalTrySplitIntoChunks(image, out chunks);
        }

        public static byte[] JoinToPng(IEnumerable<Chunk> chunks)
        {
            if (chunks is null)
            {
                throw new ArgumentNullException(nameof(chunks));
            }

            if (!chunks.Any())
            {
                throw new ArgumentException(nameof(chunks));
            }

            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);

            binaryWriter.Write(Signature);

            foreach (var chunk in chunks)
            {
                if (!chunk.IsValid())
                {
                    throw new ArgumentException();
                }

                binaryWriter.Write(chunk.Value);
            }

            return memoryStream.ToArray();
        }

        public static bool TryJoinToPng(IEnumerable<Chunk> chunks, [MaybeNullWhen(false)] out byte[] png)
        {
            if (chunks is null)
            {
                png = default;
                return false;
            }

            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);

            binaryWriter.Write(Signature);

            foreach (var chunk in chunks)
            {
                if (!chunk.IsValid())
                {
                    png = default;
                    return false;
                }

                binaryWriter.Write(chunk.Value);
            }

            png = memoryStream.ToArray();
            return true;
        }
    }
}
