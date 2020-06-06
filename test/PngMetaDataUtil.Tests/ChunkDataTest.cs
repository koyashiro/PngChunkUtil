using Microsoft.VisualStudio.TestTools.UnitTesting;
using KoyashiroKohaku.PngMetaDataUtil;
using System;
using System.Linq;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace KoyashiroKohaku.PngMetaDataUtil.Tests
{
    [TestClass]
    public class ChunkDataTest
    {
        internal static byte[] SingleByteChunkDataByteArray => new byte[]
        {
            0x54, 0x65, 0x73, 0x74, 0x20, 0x44, 0x61, 0x74, 0x61
        };
        internal static string SingleByteChunkDataString => "Test Data";
        internal static byte[] MultiByteChunkDataByteArray => new byte[]
        {
            0xE3, 0x81, 0xA6, 0xE3, 0x81, 0x99, 0xE3, 0x81, 0xA8,
            0xE3, 0x81, 0xA7, 0xE3, 0x83, 0xBC, 0xE3, 0x81, 0x9F
        };
        internal static string MultiByteChunkDataString => "てすとでーた";

        [TestMethod]
        public void Constructor_WithNoArgmentTest()
        {
            var ChunkData = new ChunkData();

            Assert.IsTrue(ChunkData.Value.SequenceEqual(new byte[8]));
        }

        [TestMethod]
        public void Constructor_WithByteSpanTest()
        {
            var ChunkData = new ChunkData(SingleByteChunkDataByteArray);

            Assert.IsTrue(ChunkData.Value.SequenceEqual(SingleByteChunkDataByteArray));
            Assert.AreEqual(SingleByteChunkDataString, ChunkData.ToString());

            ChunkData = new ChunkData(MultiByteChunkDataByteArray);

            Assert.IsTrue(ChunkData.Value.SequenceEqual(MultiByteChunkDataByteArray));
            Assert.AreEqual(MultiByteChunkDataString, ChunkData.ToString());
        }

        [TestMethod]
        public void Constructor_WithCharSpanTest()
        {
            var ChunkData = new ChunkData(SingleByteChunkDataString);

            Assert.IsTrue(ChunkData.Value.SequenceEqual(SingleByteChunkDataByteArray));
            Assert.AreEqual(SingleByteChunkDataString, ChunkData.ToString());

            ChunkData = new ChunkData(MultiByteChunkDataString);

            Assert.IsTrue(ChunkData.Value.SequenceEqual(MultiByteChunkDataByteArray));
            Assert.AreEqual(MultiByteChunkDataString, ChunkData.ToString());
        }

        [TestMethod]
        public void Constructor_SetValueWithByteSpan()
        {
            var ChunkData = new ChunkData();

            ChunkData.SetValue(SingleByteChunkDataByteArray);

            Assert.IsTrue(ChunkData.Value.SequenceEqual(SingleByteChunkDataByteArray));
            Assert.AreEqual(SingleByteChunkDataString, ChunkData.ToString());

            ChunkData.SetValue(SingleByteChunkDataByteArray);

            Assert.IsTrue(ChunkData.Value.SequenceEqual(SingleByteChunkDataByteArray));
            Assert.AreEqual(SingleByteChunkDataString, ChunkData.ToString());

            ChunkData.SetValue(MultiByteChunkDataByteArray);

            Assert.IsTrue(ChunkData.Value.SequenceEqual(MultiByteChunkDataByteArray));
            Assert.AreEqual(MultiByteChunkDataString, ChunkData.ToString());

            ChunkData.SetValue(MultiByteChunkDataByteArray);

            Assert.IsTrue(ChunkData.Value.SequenceEqual(MultiByteChunkDataByteArray));
            Assert.AreEqual(MultiByteChunkDataString, ChunkData.ToString());
        }

        [TestMethod]
        public void Constructor_SetValueWithCharSpan()
        {
            var ChunkData = new ChunkData();

            ChunkData.SetValue(SingleByteChunkDataString);

            Assert.IsTrue(ChunkData.Value.SequenceEqual(SingleByteChunkDataByteArray));
            Assert.AreEqual(SingleByteChunkDataString, ChunkData.ToString());

            ChunkData.SetValue(SingleByteChunkDataString);

            Assert.IsTrue(ChunkData.Value.SequenceEqual(SingleByteChunkDataByteArray));
            Assert.AreEqual(SingleByteChunkDataString, ChunkData.ToString());

            ChunkData.SetValue(MultiByteChunkDataString);

            Assert.IsTrue(ChunkData.Value.SequenceEqual(MultiByteChunkDataByteArray));
            Assert.AreEqual(MultiByteChunkDataString, ChunkData.ToString());

            ChunkData.SetValue(MultiByteChunkDataString);

            Assert.IsTrue(ChunkData.Value.SequenceEqual(MultiByteChunkDataByteArray));
            Assert.AreEqual(MultiByteChunkDataString, ChunkData.ToString());
        }
    }
}
