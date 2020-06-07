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
            var validChunks = ChunkReader.GetChunks(ValidImage).ToArray();

            var writedImage = ChunkWriter.WriteImage(validChunks);
            var writedChunks = ChunkReader.GetChunks(writedImage).ToArray();

            for (int i = 0; i < validChunks.Length; i++)
            {
                var writedChunk = writedChunks[i];
                var validChunk = validChunks[i];
                if (writedChunk.ChunkType.ToString() != validChunk.ChunkType.ToString())
                {
                    Assert.Fail();
                }
                if (writedChunk.ChunkData.ToString() != validChunk.ChunkData.ToString())
                {
                    Assert.Fail();
                }
            }

            for (int i = 0; i < ValidImage.Length; i++)
            {
                var writed = writedImage[i];
                var valid = ValidImage[i];
                if (writed != valid)
                {
                    Assert.Fail();
                }
            }

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
