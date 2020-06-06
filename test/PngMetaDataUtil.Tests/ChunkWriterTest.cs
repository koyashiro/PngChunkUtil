using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.IO;

namespace KoyashiroKohaku.PngMetaDataUtil.Tests
{
    [TestClass]
    public class ChunkWriterTest
    {
        private byte[] InvalidImage => File.ReadAllBytes(@"Assets/invalid.jpg");
        private byte[] ValidImage => File.ReadAllBytes(@"Assets/valid.png");

        private string[] CriticalChunks => new string[] { "IHDR", "PLTE", "IDAT", "IEND" };
        private string[] AncillaryChunks => new string[]
        {
            "cHRM", "gAMA", "iCCP", "sBIT", "sRGB", "bKGD", "hIST", "tRNS", "pHYs", "sPLT", "tIME", "iTXt", "tEXt", "zTXt"
        };

        [TestMethod]
        public void WriteImage_Test()
        {
            var chunks = ChunkReader.GetChunks(ValidImage).ToArray();

            var writedImage = ChunkWriter.WriteImage(chunks);
            var writedChunks = ChunkReader.GetChunks(writedImage);

            var result = writedChunks
                .Select(c => (c.ChunkType.ToString(), c.ChunkData.ToString()))
                .SequenceEqual(chunks.Select(c => (c.ChunkType.ToString(), c.ChunkData.ToString())));

            Assert.IsTrue(result);

            File.WriteAllBytes(@"output.png", writedImage);
        }

        [TestMethod]
        public void AddChunks_Test()
        {
            var chunks = ChunkReader.GetChunks(ValidImage);
            var appendChunks = new Chunk[]
            {
                new Chunk("TEST", "Test01"),
                new Chunk("TEST", "Test02"),
                new Chunk("TEST", "Test03"),
                new Chunk("TEST", "Test04"),
            };

            var appendedImage = ChunkWriter.AddChunk(ValidImage, appendChunks);
            var appendedChunks = ChunkReader.GetChunks(appendedImage);

            Assert.AreEqual(chunks.Count + appendChunks.Length, appendedChunks.Count);

            Assert.IsFalse(appendedChunks.All(c =>
                (c.ChunkType.ToString() == appendChunks[0].ChunkType.ToString() && c.ChunkData.ToString() == appendChunks[0].ChunkData.ToString())
                || (c.ChunkType.ToString() == appendChunks[1].ChunkType.ToString() && c.ChunkData.ToString() == appendChunks[1].ChunkData.ToString())
                || (c.ChunkType.ToString() == appendChunks[2].ChunkType.ToString() && c.ChunkData.ToString() == appendChunks[2].ChunkData.ToString())
                || (c.ChunkType.ToString() == appendChunks[3].ChunkType.ToString() && c.ChunkData.ToString() == appendChunks[3].ChunkData.ToString())
            ));
        }

        [TestMethod]
        public void RemoveChunks_Test()
        {
            var chunks = ChunkReader.GetChunks(ValidImage);
            var removedImage = ChunkWriter.RemoveChunk(ValidImage, "vrCd", "vrCw", "vrCp", "vrCu");
            var removedChunks = ChunkReader.GetChunks(removedImage);

            Assert.IsFalse(removedChunks.Any(c => c.ChunkType.ToString() == "vrCp" || c.ChunkType.ToString() == "vrCw" || c.ChunkType.ToString() == "vrCp" || c.ChunkType.ToString() == "vrCu"));
        }
    }
}
