using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.IO;

namespace KoyashiroKohaku.PngMetaDataUtil.Tests
{
    [TestClass]
    public class ChunkReaderTest
    {
        private byte[] InvalidSource => File.ReadAllBytes(@"Assets/invalid.jpg");
        private byte[] ValidSource => File.ReadAllBytes(@"Assets/valid.png");

        private string[] CriticalChunks => new string[] { "IHDR", "PLTE", "IDAT", "IEND" };
        private string[] AncillaryChunks => new string[]
        {
            "cHRM", "gAMA", "iCCP", "sBIT", "sRGB", "bKGD", "hIST", "tRNS", "pHYs", "sPLT", "tIME", "iTXt", "tEXt", "zTXt"
        };

        [TestMethod]
        public void Signature_Test()
        {
            var expected = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            var actual = ChunkReader.Signature.ToArray();

            var areEqual = expected.SequenceEqual(actual);

            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void IsPng_ArgumentNullExceptionTest()
        {
            try
            {
                ChunkReader.IsPng(null);
            }
            catch (ArgumentNullException)
            {
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void IsPng_FalseTest()
        {
            Assert.IsFalse(ChunkReader.IsPng(InvalidSource));
        }

        [TestMethod]
        public void IsPng_TrueTest()
        {
            Assert.IsTrue(ChunkReader.IsPng(ValidSource));
        }

        [TestMethod]
        public void IsPngSpan_FalseTest()
        {
            Assert.IsFalse(ChunkReader.IsPng(InvalidSource.AsSpan()));
        }

        [TestMethod]
        public void IsPngSpan_TrueTest()
        {
            Assert.IsTrue(ChunkReader.IsPng(ValidSource.AsSpan()));
        }

        [TestMethod]
        public void GetChunks_ArgumentNullExceptionTest()
        {
            try
            {
                ChunkReader.GetChunks(null);
            }
            catch (ArgumentNullException)
            {
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void GetChunks_ArgumentExceptionTest()
        {
            try
            {
                ChunkReader.GetChunks(InvalidSource);
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void GetChunks_AllChunksTest()
        {
            var chunks = ChunkReader.GetChunks(ValidSource, ChunkTypeFilter.All).ToList();

            if (chunks.Count != 291)
            {
                Assert.Fail("chunks count error.");
            }

            // 0: IHDR
            if (chunks[0].ChunkType.ToString() != "IHDR")
            {
                Assert.Fail("IHDR error.");
            }

            // 1-283: IDAT
            foreach (var chunk in chunks.GetRange(1, 283))
            {
                if (chunk.ChunkType.ToString() != "IDAT")
                {
                    Assert.Fail("IDAT error.");
                }
            }

            // 284: vrCd
            var dateChunk = chunks[284];
            if (dateChunk.ChunkType.ToString() != "vrCd")
            {
                Assert.Fail("vrCd error.");
            }
            if (dateChunk.ChunkData.ToString() != "20200603013244672")
            {
                Assert.Fail("vrCd error.");
            }

            // 285: vrCp
            var photographerChunk = chunks[285];
            if (photographerChunk.ChunkType.ToString() != "vrCp")
            {
                Assert.Fail("vrCp error.");
            }
            if (photographerChunk.ChunkData.ToString() != "KoyashiroKohaku")
            {
                Assert.Fail("vrCp error.");
            }

            // 286: vrCw
            var worldChunk = chunks[286];
            if (worldChunk.ChunkType.ToString() != "vrCw")
            {
                Assert.Fail("vrCw error.");
            }
            if (worldChunk.ChunkData.ToString() != "ミツキツネ家")
            {
                Assert.Fail("vrCw error.");
            }

            // 287-289: vrCu
            var playerChunks = chunks.GetRange(287, 3);
            if (playerChunks[0].ChunkType.ToString() != "vrCu")
            {
                Assert.Fail("vrCu error.");
            }
            if (playerChunks[0].ChunkData.ToString() != "KoyashiroKohaku")
            {
                Assert.Fail("vrCu error.");
            }
            if (playerChunks[1].ChunkType.ToString() != "vrCu")
            {
                Assert.Fail("vrCu error.");
            }
            if (playerChunks[1].ChunkData.ToString() != "gatosyocora")
            {
                Assert.Fail("vrCu error.");
            }
            if (playerChunks[2].ChunkType.ToString() != "vrCu")
            {
                Assert.Fail("vrCu error.");
            }
            if (playerChunks[2].ChunkData.ToString() != "mitsu_kitsune")
            {
                Assert.Fail("vrCu error.");
            }

            // 290: IEND
            if (chunks[290].ChunkType.ToString() != "IEND")
            {
                Assert.Fail("IEND error.");
            }
        }

        [TestMethod]
        public void GetChunks_CriticalChunkOnlyTest()
        {
            var chunks = ChunkReader.GetChunks(ValidSource, ChunkTypeFilter.CriticalChunkOnly).ToList();

            var difference = chunks.Select(c => c.ChunkType.ToString()).Except(CriticalChunks);

            Assert.IsFalse(difference.Any());
        }

        [TestMethod]
        public void GetChunks_AncillaryChunkOnlyTest()
        {
            var chunks = ChunkReader.GetChunks(ValidSource, ChunkTypeFilter.AncillaryChunkOnly).ToList();

            var difference = chunks.Select(c => c.ChunkType.ToString()).Except(AncillaryChunks);

            Assert.IsFalse(difference.Any());
        }

        [TestMethod]
        public void GetChunks_AdditionalChunkOnlyTest()
        {
            var chunks = ChunkReader.GetChunks(ValidSource, ChunkTypeFilter.AdditionalChunkOnly).ToList();

            var difference = chunks.Select(c => c.ChunkType.ToString()).Intersect(CriticalChunks.Union(AncillaryChunks));

            Assert.IsFalse(difference.Any());
        }
    }
}
