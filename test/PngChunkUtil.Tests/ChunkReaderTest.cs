using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.IO;

namespace KoyashiroKohaku.PngChunkUtil.Tests
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
            var actual = PngUtil.Signature.ToArray();

            var areEqual = expected.SequenceEqual(actual);

            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void IsPng_FalseTest()
        {
            Assert.IsFalse(PngUtil.IsPng(InvalidSource));
        }

        [TestMethod]
        public void IsPng_TrueTest()
        {
            Assert.IsTrue(PngUtil.IsPng(ValidSource));
        }

        [TestMethod]
        public void IsPngSpan_FalseTest()
        {
            Assert.IsFalse(PngUtil.IsPng(InvalidSource.AsSpan()));
        }

        [TestMethod]
        public void IsPngSpan_TrueTest()
        {
            Assert.IsTrue(PngUtil.IsPng(ValidSource.AsSpan()));
        }

        [TestMethod]
        public void SplitChunks_ArgumentNullExceptionTest()
        {
            try
            {
                ChunkReader.SplitChunks(null);
            }
            catch (ArgumentNullException)
            {
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void SplitChunks_ArgumentExceptionTest()
        {
            try
            {
                ChunkReader.SplitChunks(InvalidSource);
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void SplitChunks_AllChunksTest()
        {
            var chunks = ChunkReader.SplitChunks(ValidSource, ChunkTypeFilter.All).ToList();

            if (chunks.Count != 291)
            {
                Assert.Fail("chunks count error.");
            }

            // 0: IHDR
            if (chunks[0].TypeString != "IHDR")
            {
                Assert.Fail("IHDR error.");
            }

            // 1-283: IDAT
            foreach (var chunk in chunks.GetRange(1, 283))
            {
                if (chunk.TypeString != "IDAT")
                {
                    Assert.Fail("IDAT error.");
                }
            }

            // 284: vrCd
            var dateChunk = chunks[284];
            if (dateChunk.TypeString != "vrCd")
            {
                Assert.Fail("vrCd error.");
            }
            if (dateChunk.DataString != "20200603013244672")
            {
                Assert.Fail("vrCd error.");
            }

            // 285: vrCp
            var photographerChunk = chunks[285];
            if (photographerChunk.TypeString != "vrCp")
            {
                Assert.Fail("vrCp error.");
            }
            if (photographerChunk.DataString != "KoyashiroKohaku")
            {
                Assert.Fail("vrCp error.");
            }

            // 286: vrCw
            var worldChunk = chunks[286];
            if (worldChunk.TypeString != "vrCw")
            {
                Assert.Fail("vrCw error.");
            }
            if (worldChunk.DataString != "ミツキツネ家")
            {
                Assert.Fail("vrCw error.");
            }

            // 287-289: vrCu
            var playerChunks = chunks.GetRange(287, 3);
            if (playerChunks[0].TypeString != "vrCu")
            {
                Assert.Fail("vrCu error.");
            }
            if (playerChunks[0].DataString != "KoyashiroKohaku")
            {
                Assert.Fail("vrCu error.");
            }
            if (playerChunks[1].TypeString != "vrCu")
            {
                Assert.Fail("vrCu error.");
            }
            if (playerChunks[1].DataString != "gatosyocora")
            {
                Assert.Fail("vrCu error.");
            }
            if (playerChunks[2].TypeString != "vrCu")
            {
                Assert.Fail("vrCu error.");
            }
            if (playerChunks[2].DataString != "mitsu_kitsune")
            {
                Assert.Fail("vrCu error.");
            }

            // 290: IEND
            if (chunks[290].TypeString != "IEND")
            {
                Assert.Fail("IEND error.");
            }
        }

        [TestMethod]
        public void SplitChunks_CriticalChunkOnlyTest()
        {
            var chunks = ChunkReader.SplitChunks(ValidSource, ChunkTypeFilter.CriticalChunkOnly).ToList();

            var difference = chunks.Select(c => c.TypeString).Except(CriticalChunks);

            Assert.IsFalse(difference.Any());
        }

        [TestMethod]
        public void SplitChunks_AncillaryChunkOnlyTest()
        {
            var chunks = ChunkReader.SplitChunks(ValidSource, ChunkTypeFilter.AncillaryChunkOnly).ToList();

            var difference = chunks.Select(c => c.TypeString).Except(AncillaryChunks);

            Assert.IsFalse(difference.Any());
        }

        [TestMethod]
        public void SplitChunks_AdditionalChunkOnlyTest()
        {
            var chunks = ChunkReader.SplitChunks(ValidSource, ChunkTypeFilter.AdditionalChunkOnly).ToList();

            var difference = chunks.Select(c => c.TypeString).Intersect(CriticalChunks.Union(AncillaryChunks));

            Assert.IsFalse(difference.Any());
        }
    }
}
