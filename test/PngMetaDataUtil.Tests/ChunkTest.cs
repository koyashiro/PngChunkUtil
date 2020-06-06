using Microsoft.VisualStudio.TestTools.UnitTesting;
using KoyashiroKohaku.PngMetaDataUtil;
using System;
using System.Linq;
using System.IO;

namespace KoyashiroKohaku.PngMetaDataUtil.Tests
{
    [TestClass]
    public class ChunkTest
    {
        [TestMethod]
        public void Constructor_WithNoArgmentTest()
        {
            var chunk = new Chunk();

            Assert.IsTrue(chunk.ChunkType.Value.SequenceEqual(new byte[4]));
            Assert.IsTrue(chunk.ChunkData.Value.SequenceEqual(new byte[8]));
        }

        [TestMethod]
        public void Constructor_WithChunkTypeAndChunkData()
        {
            var chunkType = new ChunkType(ChunkTypeTest.IdatChunkTypeByteArray);
            var chunkData = new ChunkData(ChunkDataTest.SingleByteChunkDataByteArray);
            var chunk = new Chunk(chunkType, chunkData);

            Assert.AreEqual(chunk.ChunkType, chunkType);
            Assert.AreEqual(chunk.ChunkData, chunkData);
        }

        [TestMethod]
        public void Constructor_WithByteSpanTest()
        {
            var chunk = new Chunk(ChunkTypeTest.IdatChunkTypeByteArray, ChunkDataTest.SingleByteChunkDataByteArray);

            Assert.IsTrue(chunk.ChunkType.Value.SequenceEqual(ChunkTypeTest.IdatChunkTypeByteArray));
            Assert.AreEqual(chunk.ChunkType.ToString(), ChunkTypeTest.IdatChunkTypeString);

            Assert.IsTrue(chunk.ChunkData.Value.SequenceEqual(ChunkDataTest.SingleByteChunkDataByteArray));
            Assert.AreEqual(chunk.ChunkData.ToString(), ChunkDataTest.SingleByteChunkDataString);
        }

        [TestMethod]
        public void Constructor_WithCharSpanTest()
        {
            var chunk = new Chunk(ChunkTypeTest.IdatChunkTypeString, ChunkDataTest.SingleByteChunkDataString);

            Assert.IsTrue(chunk.ChunkType.Value.SequenceEqual(ChunkTypeTest.IdatChunkTypeByteArray));
            Assert.AreEqual(chunk.ChunkType.ToString(), ChunkTypeTest.IdatChunkTypeString);

            Assert.IsTrue(chunk.ChunkData.Value.SequenceEqual(ChunkDataTest.SingleByteChunkDataByteArray));
            Assert.AreEqual(chunk.ChunkData.ToString(), ChunkDataTest.SingleByteChunkDataString);
        }
    }
}
