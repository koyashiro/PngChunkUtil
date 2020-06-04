using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace KoyashiroKohaku.PngMetaDataTool
{
    public static class PngMetaDataParser
    {
        public static readonly byte[] PngSignature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        public static bool IsPng(byte[] source) => IsPng(source.AsSpan());

        public static bool IsPng(ReadOnlySpan<byte> source) => source.Slice(0, 8).SequenceEqual(PngSignature.AsSpan());

        public static IEnumerable<Chunk> GetChunks(byte[] source)
        {
            if (source is null)
            {
                throw new ArgumentNullException($"Argument error. argument: '{nameof(source)}' is null.");
            }

            var span = source.AsSpan();

            if (!span.Slice(0, 8).SequenceEqual(PngSignature.AsSpan()))
            {
                throw new ArgumentException($"Argument error. argument: '{nameof(source)}' is broken or no png image binary.");
            }

            // 先頭のシグネチャを除く
            var index = 8;

            var chunks = new List<Chunk>();

            while (index < span.Length)
            {
                var length = BinaryPrimitives.ReadInt32BigEndian(span.Slice(index, 4));
                var type = Encoding.UTF8.GetString(span.Slice(index + 4, 4));
                var data = span.Slice(index + 8, length);
                var crc = span.Slice(index + 8 + length, 4);

                chunks.Add(new Chunk
                {
                    Length = length,
                    ChunkType = type,
                    ChunkData = data.ToArray()
                });

                index += 12 + length;
            }

            return chunks;
        }
    }
}
