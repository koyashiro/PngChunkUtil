using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Force.Crc32;

namespace KoyashiroKohaku.PngMetaDataUtil
{
    public static class ChunkWriter
    {
        public static byte[] WriteImage(ReadOnlySpan<Chunk> chunks)
        {
            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);

            // [4 byte] : Signature
            binaryWriter.Write(PngUtil.Signature);

            Span<byte> length = stackalloc byte[4];
            Span<byte> crc = stackalloc byte[4];

            foreach (var chunk in chunks)
            {
                // [4 byte] : Length of ChunkData
                BinaryPrimitives.WriteInt32BigEndian(length, chunk.ChunkDataLength);
                binaryWriter.Write(length);

                // [4 byte] : ChunkType
                binaryWriter.Write(chunk.ChunkType.Value);

                // [(length) byte] : ChunkData
                binaryWriter.Write(chunk.ChunkData.Value);

                var typeAndData = new byte[(4 + chunk.ChunkDataLength)];
                chunk.ChunkType.Value.CopyTo(typeAndData.AsSpan().Slice(0, 4));
                chunk.ChunkData.Value.CopyTo(typeAndData.AsSpan().Slice(4, chunk.ChunkDataLength));

                // [4 byte] : CRC
                BinaryPrimitives.WriteUInt32BigEndian(crc, Crc32Algorithm.Compute(typeAndData));
                binaryWriter.Write(crc);
            }

            return memoryStream.ToArray();
        }

        public static byte[] AddChunk(ReadOnlySpan<byte> image, params Chunk[] appendChunks)
        {
            var chunks = ChunkReader.GetChunks(image);
            chunks.InsertRange(chunks.Count - 2, appendChunks);

            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);

            binaryWriter.Write(PngUtil.Signature);

            var length = new byte[4];

            foreach (var chunk in chunks)
            {
                // [4 byte] : Length of ChunkData
                BinaryPrimitives.WriteInt32BigEndian(length, chunk.ChunkDataLength);
                binaryWriter.Write(length);

                // [4 byte] : ChunkType
                binaryWriter.Write(chunk.ChunkType.Value);

                // [(length) byte] : ChunkData
                binaryWriter.Write(chunk.ChunkData.Value);

                // [4 byte] : CRC
                binaryWriter.Write(new byte[4]);
            }

            return memoryStream.ToArray();
        }

        public static byte[] RemoveChunk(ReadOnlySpan<byte> image, params string[] chunkTypes)
        {
            var chunks = ChunkReader.GetChunks(image).Where(c => !chunkTypes.Any(ct => ct == c.ChunkType.ToString())).ToArray();

            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);

            binaryWriter.Write(PngUtil.Signature);

            var length = new byte[4];

            foreach (var chunk in chunks)
            {
                // [4 byte] : Length of ChunkData
                BinaryPrimitives.WriteInt32BigEndian(length, chunk.ChunkDataLength);
                binaryWriter.Write(length);

                // [4 byte] : ChunkType
                binaryWriter.Write(chunk.ChunkType.Value);

                // [(length) byte] : ChunkData
                binaryWriter.Write(chunk.ChunkData.Value);

                // [4 byte] : CRC
                binaryWriter.Write(new byte[4]);
            }

            //for (int i = 0; i < chunks.Length; i++)
            //{
            //    var chunk = chunks[i];

            //    // [4 byte] : Length of ChunkData
            //    BinaryPrimitives.WriteInt32BigEndian(length, chunk.ChunkDataLength);
            //    binaryWriter.Write(length);

            //    // [4 byte] : ChunkType
            //    binaryWriter.Write(chunk.ChunkType.Value);

            //    // [(length) byte] : ChunkData
            //    binaryWriter.Write(chunk.ChunkData.Value);

            //    if (i+1 != chunks.Length)
            //    {
            //        // [4 byte] : CRC
            //        binaryWriter.Write(new byte[4]);
            //    }
            //}

            return memoryStream.ToArray();
        }
    }
}
