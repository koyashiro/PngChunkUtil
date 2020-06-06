using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;

namespace KoyashiroKohaku.PngMetaDataUtil
{
    /// <summary>
    /// ChunkReader
    /// </summary>
    public static class ChunkReader
    {
        /// <summary>
        /// PNG画像のシグネチャ
        /// </summary>
        public static ReadOnlySpan<byte> Signature => new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

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

            return source.Slice(0, 8).SequenceEqual(Signature);
        }

        /// <summary>
        /// PNG画像から'IHDR', 'IDAT', 'IEND'を除くチャンクをすべて抽出します。
        /// </summary>
        /// <param name = "source" > PNG画像のbyte配列 </ param >
        /// < param name="expression">抽出条件</param>
        /// <returns></returns>
        public static List<Chunk> GetChunks(byte[] source, ChunkTypeFilter chunkTypeFilter = ChunkTypeFilter.All)
        {
            if (source is null)
            {
                throw new ArgumentNullException($"argument error. argument: '{nameof(source)}' is null.");
            }

            if (!Enum.IsDefined(typeof(ChunkTypeFilter), chunkTypeFilter))
            {
                throw new ArgumentException();
            }

            var span = source.AsSpan();

            if (!span.Slice(0, 8).SequenceEqual(Signature))
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

                if (!IsTargetChunk(type, chunkTypeFilter))
                {
                    // to next chunk
                    index += 4 + 4 + length + 4;

                    continue;
                }

                // [(length) byte] : ChunkData
                var data = span.Slice(index + 8, length);

                chunks.Add(new Chunk(type, data));

                // to next chunk
                index += 4 + 4 + length + 4;
            }

            return chunks;
        }

        public static bool IsTargetChunk(ReadOnlySpan<byte> chunkType, ChunkTypeFilter chunkTypeFilter)
        {
            if (chunkType == null)
            {
                throw new ArgumentNullException();
            }

            if (chunkType.Length != 4)
            {
                throw new ArgumentException();
            }

            if (!Enum.IsDefined(typeof(ChunkTypeFilter), chunkTypeFilter))
            {
                throw new ArgumentException();
            }

            return chunkTypeFilter switch
            {
                ChunkTypeFilter.All => true,
                ChunkTypeFilter.CriticalChunkOnly => ChunkUtil.IsCriticalChunk(chunkType),
                ChunkTypeFilter.AncillaryChunkOnly => ChunkUtil.IsAncillaryChunk(chunkType),
                ChunkTypeFilter.AdditionalChunkOnly => ChunkUtil.IsAdditionalChunk(chunkType),
                ChunkTypeFilter.WithoutCriticalChunk => !ChunkUtil.IsCriticalChunk(chunkType),
                ChunkTypeFilter.WithoutAncillaryChunk => !ChunkUtil.IsAncillaryChunk(chunkType),
                ChunkTypeFilter.WithoutAdditionalChunk => !ChunkUtil.IsAdditionalChunk(chunkType),
                _ => false
            };
        }
    }
}
