using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace KoyashiroKohaku.PngMetaDataTool
{
    /// <summary>
    /// MetaDataParser
    /// </summary>
    public static class PngMetaDataParser
    {
        /// <summary>
        /// PNG画像のシグネチャ
        /// </summary>
        public static readonly byte[] PngSignature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        /// <summary>
        /// PNG画像かどうかをチェックします。
        /// </summary>
        /// <param name="source">PNG画像のbyte配列</param>
        /// <returns></returns>
        public static bool IsPng(byte[] source)
        {
            if (source == null)
            {
                throw new ArgumentNullException($"argument error. argument: '{nameof(source)}' is null.");
            }

            return IsPng(source.AsSpan());
        }

        /// <summary>
        /// PNG画像かどうかをチェックします。
        /// </summary>
        /// <param name="source">PNG画像のSpan構造体</param>
        /// <returns></returns>
        public static bool IsPng(ReadOnlySpan<byte> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException($"argument error. argument: '{nameof(source)}' is null.");
            }

            if (source.Length < 8)
            {
                return false;
            }

            return source.Slice(0, 8).SequenceEqual(PngSignature.AsSpan());
        }

        /// <summary>
        /// PNG画像からすべてのチャンクを抽出します。
        /// </summary>
        /// <param name="source">PNG画像のbyte配列</param>
        /// <returns></returns>
        public static IEnumerable<Chunk> GetAllChunks(byte[] source)
        {
            if (source is null)
            {
                throw new ArgumentNullException($"argument error. argument: '{nameof(source)}' is null.");
            }

            var span = source.AsSpan();

            if (!span.Slice(0, 8).SequenceEqual(PngSignature.AsSpan()))
            {
                throw new ArgumentException($"argument error. argument: '{nameof(source)}' is broken or no png image binary.");
            }

            // 先頭のシグネチャを除く
            var index = 8;

            var chunks = new List<Chunk>();

            while (index < span.Length)
            {
                // [4 byte] : Length of ChunkData
                var length = BinaryPrimitives.ReadInt32BigEndian(span.Slice(index, 4));

                // [4 byte] : ChunkType
                var type = span.Slice(index + 4, 4);

                // [(length) byte] : ChunkData
                var data = span.Slice(index + 8, length);

                // [4 byte] : crc (not used)
                // var crc = span.Slice(index + 8 + length, 4);

                chunks.Add(new Chunk
                {
                    ChunkType = new ChunkType(type),
                    ChunkData = new ChunkData(data)
                });

                // to next chunk
                index += 4 + 4 + length + 4;
            }

            return chunks;
        }

        /// <summary>
        /// PNG画像から条件に合致するチャンクをすべて抽出します。
        /// </summary>
        /// <param name="source">PNG画像のbyte配列</param>
        /// <param name="expression">抽出条件</param>
        /// <returns></returns>
        public static IEnumerable<Chunk> GetChunks(byte[] source, Expression<Func<ChunkType, bool>> expression)
        {
            if (source is null)
            {
                throw new ArgumentNullException($"argument error. argument: '{nameof(source)}' is null.");
            }

            var span = source.AsSpan();

            if (!span.Slice(0, 8).SequenceEqual(PngSignature.AsSpan()))
            {
                throw new ArgumentException($"argument error. argument: '{nameof(source)}' is broken or no png image binary.");
            }

            // 先頭のシグネチャを除く
            var index = 8;

            var chunks = new List<Chunk>();

            while (index < span.Length)
            {
                // [4 byte] : Length of ChunkData
                var length = BinaryPrimitives.ReadInt32BigEndian(span.Slice(index, 4));

                // [4 byte] : ChunkType
                var chunkType = new ChunkType(span.Slice(index + 4, 4));

                // expressionで条件に合致するかチェック
                if (!expression.Compile()(chunkType))
                {
                    // to next chunk
                    index += 4 + 4 + length + 4;

                    continue;
                }

                // [(length) byte] : ChunkData
                var data = span.Slice(index + 8, length);

                chunks.Add(new Chunk
                {
                    ChunkType = chunkType,
                    ChunkData = new ChunkData(data)
                });

                // [4 byte] : crc (not used)
                // var crc = span.Slice(index + 8 + length, 4);

                // to next chunk
                index += 4 + 4 + length + 4;
            }

            return chunks;
        }
    }
}
