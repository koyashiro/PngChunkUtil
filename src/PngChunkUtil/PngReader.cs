using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace KoyashiroKohaku.PngChunkUtil
{
    public static class PngReader
    {
        private static bool HasSignature(ReadOnlySpan<byte> source)
        {
            if (source.Length < 8)
            {
                return false;
            }

            return source[0..8].SequenceEqual(Png.Signature);
        }

        public static Chunk[] Parse(ReadOnlyMemory<byte> input)
        {
            if (!HasSignature(input.Span))
            {
                throw new ArgumentException("invalid signature", nameof(input));
            }

            var chunks = new List<Chunk>();

            var index = 8;
            while (index < input.Length)
            {
                if (index + 8 > input.Length)
                {
                    throw new ArgumentException("invalid png", nameof(input));
                }

                var length = BinaryPrimitives.ReadInt32BigEndian(input.Span[index..(index + 4)]) + 12;

                if (index + length > input.Length)
                {
                    throw new ArgumentException("invalid png", nameof(input));
                }

                var chunk = Chunk.Parse(input[index..(index + length)]);
                if (!chunk.IsValid())
                {
                    throw new ArgumentException("invalid png", nameof(input));
                }

                chunks.Add(chunk);

                if (CriticalChunk.IEND.SequenceEqual(chunk.ChunkTypeBytes))
                {
                    break;
                }

                index += length;
            }

            return chunks.ToArray();
        }

        public static bool TryParse(ReadOnlyMemory<byte> input, [MaybeNullWhen(false)] out Chunk[] chunks)
        {
            if (!HasSignature(input.Span))
            {
                chunks = default;
                return false;
            }

            var list = new List<Chunk>();

            var index = 8;
            while (index < input.Length)
            {
                if (index + 8 > input.Length)
                {
                    chunks = default;
                    return false;
                }

                var length = BinaryPrimitives.ReadInt32BigEndian(input.Span[index..(index + 4)]) + 12;

                if (index + length > input.Length)
                {
                    chunks = default;
                    return false;
                }

                var chunk = Chunk.Parse(input[index..(index + length)]);
                if (!chunk.IsValid())
                {
                    chunks = default;
                    return false;
                }

                list.Add(chunk);

                if (CriticalChunk.IEND.SequenceEqual(chunk.ChunkTypeBytes))
                {
                    break;
                }

                index += length;
            }


            chunks = list.ToArray();
            return true;
        }
    }
}
