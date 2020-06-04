using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace KoyashiroKohaku.PngMetaDataTool
{
    public class Chunk
    {
        public int Length { get; set; }
        public string ChunkType { get; set; }
        public byte[] ChunkData { get; set; }

        public override string ToString()
        {
            return $"{ChunkType}: {Encoding.UTF8.GetString(ChunkData)}";
        }
    }
}
