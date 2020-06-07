using Microsoft.VisualStudio.TestTools.UnitTesting;
using KoyashiroKohaku.PngMetaDataUtil;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace KoyashiroKohaku.PngMetaDataUtil.Tests
{
    [TestClass]
    public class ChunkTest
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
        internal static byte[] IhdrChunkTypeByteArray => new byte[] { 0x49, 0x48, 0x44, 0x52 };
        internal static string IhdrChunkTypeString => "IHDR";
        internal static byte[] IdatChunkTypeByteArray => new byte[] { 0x49, 0x44, 0x41, 0x54 };
        internal static string IdatChunkTypeString => "IDAT";

        [TestMethod]
        public void Constructor_WithNoArgmentTest()
        {
            var chunk = new Chunk();
           
            Assert.IsTrue(chunk.TypePart.SequenceEqual(new byte[4]));
            Assert.IsTrue(chunk.DataPart.SequenceEqual(Enumerable.Empty<byte>().ToArray()));
        }

        [TestMethod]
        public void Constructor_WithByteSpanTest()
        {
            var chunk = new Chunk(IdatChunkTypeByteArray, SingleByteChunkDataByteArray);

            Assert.IsTrue(chunk.TypePart.SequenceEqual(IdatChunkTypeByteArray));
            Assert.AreEqual(chunk.TypeString, IdatChunkTypeString);

            Assert.IsTrue(chunk.DataPart.SequenceEqual(SingleByteChunkDataByteArray));
            Assert.AreEqual(chunk.DataString, SingleByteChunkDataString);
        }

        [TestMethod]
        public void Constructor_WithCharSpanTest()
        {
            var chunk = new Chunk(IdatChunkTypeString, SingleByteChunkDataString);

            Assert.IsTrue(chunk.TypePart.SequenceEqual(IdatChunkTypeByteArray));
            Assert.AreEqual(chunk.TypeString, IdatChunkTypeString);

            Assert.IsTrue(chunk.DataPart.SequenceEqual(SingleByteChunkDataByteArray));
            Assert.AreEqual(chunk.DataString, SingleByteChunkDataString);
        }
    }
}
