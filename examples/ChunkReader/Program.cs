using System;
using System.IO;
using System.Linq;

using Koyashiro.PngChunkUtil;

namespace Koyashiro.ChunkReader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.WriteLine("path is required");
                Environment.Exit(1);
            }

            var path = args[0];
            var bytes = File.ReadAllBytes(path);
            var chunks = PngReader.Parse(bytes);

            foreach (var chunk in chunks)
            {
                var length = string.Format("{0,5}", chunk.Length);
                var chunkType = chunk.ChunkType;
                var chunkData = string.Join(", ", chunk.ChunkDataBytes.ToArray().Select(b => $"0x{b:x2}"));

                Console.WriteLine($"Length: {length},    Chunk Type: {chunkType},    Chunk Data: [ {(chunkData.Length > 48 ? $"{chunkData.Remove(48)}..." : chunkData)} ]");
            }
        }
    }
}
