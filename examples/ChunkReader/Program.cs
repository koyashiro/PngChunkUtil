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
                Console.WriteLine(chunk);
            }
        }
    }
}
