using KoyashiroKohaku.PngChunkUtil.Properties;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

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
                throw new ArgumentException(Resources.ChunkReader_SplitChunk_InvalidImage, nameof(image));
            }

            // 先頭のシグネチャを除く
            var index = 8;

            var chunks = new List<Chunk>();

            while (index < image.Length)
            {
                if (index + 8 > image.Length)
                {
                    throw new ArgumentException(Resources.ChunkReader_SplitChunk_InvalidChunk, nameof(image));
                }

                // [4 byte] : Length of ChunkData
                var length = BinaryPrimitives.ReadInt32BigEndian(image.Slice(index, 4));

                if (index + 12 + length > image.Length)
                {
                    throw new ArgumentException(Resources.ChunkReader_SplitChunk_InvalidChunk, nameof(image));
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
                    throw new ArgumentException(Resources.ChunkReader_SplitChunk_InvalidChunk, nameof(image));
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

        /// <summary>
        /// PNG画像からチャンクを抽出します。
        /// </summary>
        /// <param name="image"></param>
        /// <param name="chunkTypeFilter"></param>
        /// <returns></returns>
        public static Task<List<Chunk>> SplitChunksAsync(ReadOnlySpan<byte> image, ChunkTypeFilter chunkTypeFilter = ChunkTypeFilter.All)
        {
            var buffer = image.ToArray();
            return Task.Run(() => SplitChunks(buffer, chunkTypeFilter));
        }

        /// <summary>
        /// PNG画像からチャンクを抽出します。
        /// </summary>
        /// <param name="image"></param>
        /// <param name="chunks"></param>
        /// <param name="chunkTypeFilter"></param>
        /// <returns></returns>
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

        /// <summary>
        /// PNG画像からチャンクを抽出します。
        /// </summary>
        /// <param name="image"></param>
        /// <param name="chunkTypeFilter"></param>
        /// <returns></returns>
        public static Task<(bool isSuccess, List<Chunk>? chunks)> TrySplitChunksAsync(byte[] image, ChunkTypeFilter chunkTypeFilter = ChunkTypeFilter.All)
        {
            return Task.Run(() =>
            {
                var result = TrySplitChunks(image.ToArray(), out var chunks, chunkTypeFilter);
                return (result, chunks);
            });
        }

        /// <summary>
        /// 抽出対象のチャンクかどうかをチェックします。
        /// </summary>
        /// <param name="chunkType"></param>
        /// <param name="chunkTypeFilter"></param>
        /// <returns></returns>
        private static bool IsTargetChunk(ReadOnlySpan<byte> chunkType, ChunkTypeFilter chunkTypeFilter)
        {
            if (chunkType.Length != 4)
            {
                throw new ArgumentException(Resources.Chunk_Constructor_ChunkTypeOutOfRange, nameof(chunkType));
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
