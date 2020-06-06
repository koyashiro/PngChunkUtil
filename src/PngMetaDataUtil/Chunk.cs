using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace KoyashiroKohaku.PngMetaDataUtil
{
    /// <summary>
    /// Chunk
    /// </summary>
    public class Chunk
    {
        public Chunk()
        {
            ChunkType = new ChunkType();
            ChunkData = new ChunkData();
        }

        public Chunk(ChunkType chunkType, ChunkData chunkData)
        {
            ChunkType = chunkType;
            ChunkData = chunkData;
        }

        public Chunk(ReadOnlySpan<byte> type, ReadOnlySpan<byte> data)
        {
            ChunkType = new ChunkType(type);
            ChunkData = new ChunkData(data);
        }

        public Chunk(ReadOnlySpan<char> type, ReadOnlySpan<char> data)
        {
            ChunkType = new ChunkType(type);
            ChunkData = new ChunkData(data);
        }

        /// <summary>
        /// ChunkTypeを返却します。
        /// </summary>
        public ChunkType ChunkType { get; }

        /// <summary>
        /// ChunkDataを返却します。
        /// </summary>
        public ChunkData ChunkData { get; }

        /// <summary>
        /// ChunkDataのバイト長を返却します。
        /// </summary>
        public int ChunkDataLength => ChunkData.Length;

        public override string ToString()
        {
            return $"{ChunkType}: {ChunkData}";
        }
    }
}
