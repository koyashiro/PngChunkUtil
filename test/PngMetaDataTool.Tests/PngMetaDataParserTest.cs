using Microsoft.VisualStudio.TestTools.UnitTesting;
using KoyashiroKohaku.PngMetaDataTool;
using System;
using System.Linq;
using System.IO;

namespace KoyashiroKohaku.PngMetaDataTool.Tests
{
    [TestClass]
    public class PngMetaDataParserTest
    {
        private byte[] InvalidSource => File.ReadAllBytes(@"Assets/invalid.jpg");
        private byte[] ValidSource => File.ReadAllBytes(@"Assets/valid.png");

        [TestMethod]
        public void PngSignature_Test()
        {
            var expected = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            var actual = PngMetaDataParser.PngSignature.ToArray();

            var areEqual = expected.SequenceEqual(actual);

            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void IsPng_ArgumentNullExceptionTest()
        {
            try
            {
                PngMetaDataParser.IsPng(null);
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
            Assert.IsFalse(PngMetaDataParser.IsPng(InvalidSource));
        }

        [TestMethod]
        public void IsPng_TrueTest()
        {
            Assert.IsTrue(PngMetaDataParser.IsPng(ValidSource));
        }

        [TestMethod]
        public void GetAllChunks_ArgumentNullExceptionTest()
        {
            try
            {
                PngMetaDataParser.GetAllChunks(null);
            }
            catch (ArgumentNullException)
            {
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void GetAllChunks_Test()
        {
            var chunks = PngMetaDataParser.GetAllChunks(ValidSource).ToList();

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
        public void GetAllChunks_ArgumentExceptionTest()
        {
            try
            {
                PngMetaDataParser.GetAllChunks(InvalidSource);
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void GetChunks_ArgumentNullExceptionTest()
        {
            try
            {
                PngMetaDataParser.GetChunks(null, null);
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
                PngMetaDataParser.GetChunks(InvalidSource, null);
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void GetChunks_Test()
        {
            var chunks = PngMetaDataParser.GetChunks(ValidSource, ct => ct.ToString() != "IHDR" && ct.ToString() != "IDAT" && ct.ToString() != "IEND").ToList();

            if (chunks.Count != 6)
            {
                Assert.Fail("chunks count error.");
            }

            // 0: vrCd
            var dateChunk = chunks[0];
            if (dateChunk.ChunkType.ToString() != "vrCd")
            {
                Assert.Fail("vrCd error.");
            }
            if (dateChunk.ChunkData.ToString() != "20200603013244672")
            {
                Assert.Fail("vrCd error.");
            }

            // 1: vrCp
            var photographerChunk = chunks[1];
            if (photographerChunk.ChunkType.ToString() != "vrCp")
            {
                Assert.Fail("vrCp error.");
            }
            if (photographerChunk.ChunkData.ToString() != "KoyashiroKohaku")
            {
                Assert.Fail("vrCp error.");
            }

            // 2: vrCw
            var worldChunk = chunks[2];
            if (worldChunk.ChunkType.ToString() != "vrCw")
            {
                Assert.Fail("vrCw error.");
            }
            if (worldChunk.ChunkData.ToString() != "ミツキツネ家")
            {
                Assert.Fail("vrCw error.");
            }

            // 3-5: vrCu
            var playerChunks = chunks.GetRange(3, 3);
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
        }
    }
}
