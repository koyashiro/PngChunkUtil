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
                if (writedChunk.TypeString != validChunk.TypeString)
                {
                    Assert.Fail();
                }
                if (writedChunk.DataString != validChunk.DataString)
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
                (c.TypeString == appendChunks[0].TypeString && c.DataString == appendChunks[0].DataString)
                || (c.TypeString == appendChunks[1].TypeString && c.DataString == appendChunks[1].DataString)
                || (c.TypeString == appendChunks[2].TypeString && c.DataString == appendChunks[2].DataString)
                || (c.TypeString == appendChunks[3].TypeString && c.DataString == appendChunks[3].DataString)
            ));
        }

        [TestMethod]
        public void RemoveChunks_Test()
        {
            var chunks = ChunkReader.GetChunks(ValidImage);
            var removedImage = ChunkWriter.RemoveChunk(ValidImage, "vrCd", "vrCw", "vrCp", "vrCu");
            var removedChunks = ChunkReader.GetChunks(removedImage);

            Assert.IsFalse(removedChunks.Any(c => c.TypeString == "vrCp" || c.TypeString == "vrCw" || c.TypeString == "vrCp" || c.TypeString == "vrCu"));
        }
    }
}
