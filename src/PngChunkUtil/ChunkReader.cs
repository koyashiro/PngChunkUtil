using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace KoyashiroKohaku.PngChunkUtil
{
    /// <summary>
    /// ChunkReader
    /// </summary>
    public static class ChunkReader
    {
        /// <summary>
        /// PNG画像からチャンクを抽出します。
        /// </summary>
        /// <param name = "image" > PNG画像のbyte配列 </ param >
        /// < param name="expression">抽出条件</param>
        /// <returns></returns>
        public static List<Chunk> SplitChunks(ReadOnlySpan<byte> image, ChunkTypeFilter chunkTypeFilter = ChunkTypeFilter.All)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (!Enum.IsDefined(typeof(ChunkTypeFilter), chunkTypeFilter))
            {
                throw new InvalidEnumArgumentException(nameof(chunkTypeFilter), (int)chunkTypeFilter, typeof(ChunkTypeFilter));
            }

            if (!PngUtil.IsPng(image))
            {
                throw new ArgumentException($"argument error. argument: '{nameof(image)}' is broken or no png image binary.");
            }

            // 先頭のシグネチャを除く
            var index = 8;

            var chunks = new List<Chunk>();

            while (index < image.Length)
            {
                if (index + 8 > image.Length)
                {
                    throw new ArgumentException($"argument error. argument: '{nameof(image)}' is broken or no png image binary.");
                }

                // [4 byte] : Length of ChunkData
                var length = BinaryPrimitives.ReadInt32BigEndian(image.Slice(index, 4));

                if (index + 12 + length > image.Length)
                {
                    throw new ArgumentException($"argument error. argument: '{nameof(image)}' is broken or no png image binary.");
                }

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

                if (index + 8 + length + 4 > image.Length)
                {
                    throw new ArgumentException($"argument error. argument: '{nameof(image)}' is broken or no png image binary.");
                }

                // [(length) byte] : CRC (not used)
                var crc = image.Slice(index + 8 + length, 4);

                var chunk = new Chunk(type, data);

                // CRC check
                if (!crc.SequenceEqual(chunk.CrcPart))
                {
                    throw new Exception();
                }

                chunks.Add(chunk);

                // to next chunk
                index += 4 + 4 + length + 4;
            }

            return chunks;
        }

        public static bool TrySplitChunks(ReadOnlySpan<byte> image, out List<Chunk>? chunks, ChunkTypeFilter chunkTypeFilter = ChunkTypeFilter.All)
        {
            if (image == null)
            {
                chunks = null;
                return false;
            }

            if (!Enum.IsDefined(typeof(ChunkTypeFilter), chunkTypeFilter))
            {
                chunks = null;
                return false;
            }

            if (!PngUtil.IsPng(image))
            {
                chunks = null;
                return false;
            }

            // 先頭のシグネチャを除く
            var index = 8;

            chunks = new List<Chunk>();

            while (index < image.Length)
            {
                if (index + 8 > image.Length)
                {
                    chunks = null;
                    return false;
                }

                // [4 byte] : Length of ChunkData
                var length = BinaryPrimitives.ReadInt32BigEndian(image.Slice(index, 4));

                if (index + 12 + length > image.Length)
                {
                    chunks = null;
                    return false;
                }

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

                if (index + 8 + length + 4 > image.Length)
                {
                    chunks = null;
                    return false;
                }

                // [(length) byte] : CRC (not used)
                var crc = image.Slice(index + 8 + length, 4);

                var chunk = new Chunk(type, data);

                // CRC check
                if (!crc.SequenceEqual(chunk.CrcPart))
                {
                    chunks = null;
                    return false;
                }

                chunks.Add(chunk);

                // to next chunk
                index += 4 + 4 + length + 4;
            }

            return true;
        }

        public static bool IsTargetChunk(ReadOnlySpan<byte> chunkType, ChunkTypeFilter chunkTypeFilter)
        {
            if (chunkType == null)
            {
                throw new ArgumentNullException(nameof(chunkType));
            }

            if (chunkType.Length != 4)
            {
                throw new ArgumentException();
            }

            if (!Enum.IsDefined(typeof(ChunkTypeFilter), chunkTypeFilter))
            {
                throw new InvalidEnumArgumentException(nameof(chunkTypeFilter), (int)chunkTypeFilter, typeof(ChunkTypeFilter));
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
