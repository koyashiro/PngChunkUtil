using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace KoyashiroKohaku.PngMetaDataTool
{
    /// <summary>
    /// Chunk
    /// </summary>
    public class Chunk
    {
        /// <summary>
        /// ChunkTypeを返却します。
        /// </summary>
        public ChunkType ChunkType { get; set; }

        /// <summary>
        /// ChunkDataを返却します。
        /// </summary>
        public ChunkData ChunkData { get; set; }

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
