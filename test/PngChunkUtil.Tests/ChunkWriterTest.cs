using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.IO;

namespace KoyashiroKohaku.PngChunkUtil.Tests
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
            var validChunks = ChunkReader.SplitChunks(ValidImage).ToArray();

            var writedImage = ChunkWriter.WriteImageBytes(validChunks);
            var writedChunks = ChunkReader.SplitChunks(writedImage).ToArray();

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

            Assert.IsTrue(writedImage.SequenceEqual(ValidImage));

            File.WriteAllBytes(@"output.png", writedImage);
        }
    }
}
