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

            return source.Slice(0, 8).SequenceEqual(PngUtil.Signature);
        }

        /// <summary>
        /// PNG画像から'IHDR', 'IDAT', 'IEND'を除くチャンクをすべて抽出します。
        /// </summary>
        /// <param name = "image" > PNG画像のbyte配列 </ param >
        /// < param name="expression">抽出条件</param>
        /// <returns></returns>
        public static List<Chunk> GetChunks(ReadOnlySpan<byte> image, ChunkTypeFilter chunkTypeFilter = ChunkTypeFilter.All)
        {
            if (image == null)
            {
                throw new ArgumentNullException($"argument error. argument: '{nameof(image)}' is null.");
            }

            if (!Enum.IsDefined(typeof(ChunkTypeFilter), chunkTypeFilter))
            {
                throw new ArgumentException();
            }

            if (!image.Slice(0, 8).SequenceEqual(PngUtil.Signature))
            {
                throw new ArgumentException($"argument error. argument: '{nameof(image)}' is broken or no png image binary.");
            }

            // 先頭のシグネチャを除く
            var index = 8;

            var chunks = new List<Chunk>();

            while (index < image.Length)
            {
                // [4 byte] : Length of ChunkData
                var length = BinaryPrimitives.ReadInt32BigEndian(image.Slice(index, 4));

                // [4 byte] : ChunkType
                var type = image.Slice(index + 4, 4);

                if (!IsTargetChunk(type, chunkTypeFilter))
                {
                    // to next chunk
                    index += 4 + 4 + length + 4;

                    continue;
                }

                // [(length) byte] : ChunkData
                var data = image.Slice(index + 8, length);

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
