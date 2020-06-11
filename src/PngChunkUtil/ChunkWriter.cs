using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KoyashiroKohaku.PngChunkUtil.Properties;

namespace KoyashiroKohaku.PngChunkUtil
{
    public static class ChunkWriter
    {
        public static byte[] WriteImageBytes(IEnumerable<Chunk> chunks)
        {
            if (chunks == null)
            {
                throw new ArgumentNullException(nameof(chunks));
            }

            if (chunks.Any(c => c == null))
            {
                throw new ArgumentException(Resources.ChunkWriter_WriteImageBytes_ChunkIsNull, nameof(chunks));
            }

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
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (appendChunks == null)
            {
                throw new ArgumentNullException(nameof(appendChunks));
            }

            if (appendChunks.Any(c => c == null))
            {
                throw new ArgumentException(Resources.ChunkWriter_WriteImageBytes_ChunkIsNull, nameof(appendChunks));
            }

            return AddChunk(image, appendChunks);
        }

        public static byte[] AddChunk(ReadOnlySpan<byte> image, IList<Chunk> appendChunks)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (appendChunks == null)
            {
                throw new ArgumentNullException(nameof(appendChunks));
            }

            if (appendChunks.Any(c => c == null))
            {
                throw new ArgumentException(Resources.ChunkWriter_WriteImageBytes_ChunkIsNull, nameof(appendChunks));
            }

            var chunks = ChunkReader.SplitChunks(image);
            chunks.InsertRange(chunks.Count - 1, appendChunks);

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

        public static byte[] RemoveChunk(ReadOnlySpan<byte> image, params string[] chunkTypes)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (chunkTypes == null)
            {
                throw new ArgumentNullException(nameof(chunkTypes));
            }

            if (chunkTypes.Any(c => string.IsNullOrEmpty(c)))
            {
                throw new ArgumentException(Resources.Chunk_Constructor_ChunkTypeOutOfRange, nameof(chunkTypes));
            }

            var chunks = ChunkReader.SplitChunks(image).Where(c => !chunkTypes.Any(ct => ct == c.TypeString)).ToArray();

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
    }
}
