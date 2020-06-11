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
    }
}
