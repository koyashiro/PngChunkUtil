using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Force.Crc32;

namespace KoyashiroKohaku.PngChunkUtil
{
    public static class ChunkWriter
    {
        public static byte[] WriteImage(ReadOnlySpan<Chunk> chunks)
        {
            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);

            // [4 byte] : Signature
            binaryWriter.Write(PngUtil.Signature);

            foreach (var chunk in chunks)
            {
                binaryWriter.Write(chunk.Value);
            }

            return memoryStream.ToArray();
        }

        public static byte[] AddChunk(ReadOnlySpan<byte> image, params Chunk[] appendChunks)
        {
            var chunks = ChunkReader.GetChunks(image);
            chunks.InsertRange(chunks.Count - 1, appendChunks);

            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);

            binaryWriter.Write(PngUtil.Signature);

            var length = new byte[4];

            foreach (var chunk in chunks)
            {
                binaryWriter.Write(chunk.Value);
            }

            return memoryStream.ToArray();
        }

        public static byte[] RemoveChunk(ReadOnlySpan<byte> image, params string[] chunkTypes)
        {
            var chunks = ChunkReader.GetChunks(image).Where(c => !chunkTypes.Any(ct => ct == c.TypeString)).ToArray();

            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);

            binaryWriter.Write(PngUtil.Signature);

            var length = new byte[4];

            foreach (var chunk in chunks)
            {
                binaryWriter.Write(chunk.Value);
            }

            return memoryStream.ToArray();
        }
    }
}
