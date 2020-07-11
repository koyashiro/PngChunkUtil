using Microsoft.VisualStudio.TestTools.UnitTesting;
using KoyashiroKohaku.PngChunkUtil;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace KoyashiroKohaku.PngChunkUtil.Tests
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
            var singleByteChunk = new Chunk(IdatChunkTypeByteArray, SingleByteChunkDataByteArray);
            var multiByteChunk = new Chunk(IdatChunkTypeByteArray, MultiByteChunkDataByteArray);

            Assert.IsTrue(singleByteChunk.TypePart.SequenceEqual(IdatChunkTypeByteArray));
            Assert.AreEqual(singleByteChunk.TypeString, IdatChunkTypeString);

            Assert.IsTrue(singleByteChunk.DataPart.SequenceEqual(SingleByteChunkDataByteArray));
            Assert.AreEqual(singleByteChunk.DataString, SingleByteChunkDataString);

            Assert.IsTrue(multiByteChunk.TypePart.SequenceEqual(IdatChunkTypeByteArray));
            Assert.AreEqual(multiByteChunk.TypeString, IdatChunkTypeString);

            Assert.IsTrue(multiByteChunk.DataPart.SequenceEqual(MultiByteChunkDataByteArray));
            Assert.AreEqual(multiByteChunk.DataString, MultiByteChunkDataString);
        }

        [TestMethod]
        public void Constructor_WithCharSpanTest()
        {
            var singleByteChunk = new Chunk(IdatChunkTypeString, SingleByteChunkDataString);
            var multiByteChunk = new Chunk(IdatChunkTypeString, MultiByteChunkDataString);

            Assert.IsTrue(singleByteChunk.TypePart.SequenceEqual(IdatChunkTypeByteArray));
            Assert.AreEqual(singleByteChunk.TypeString, IdatChunkTypeString);

            Assert.IsTrue(singleByteChunk.DataPart.SequenceEqual(SingleByteChunkDataByteArray));
            Assert.AreEqual(singleByteChunk.DataString, SingleByteChunkDataString);

            Assert.IsTrue(multiByteChunk.TypePart.SequenceEqual(IdatChunkTypeByteArray));
            Assert.AreEqual(multiByteChunk.TypeString, IdatChunkTypeString);

            Assert.IsTrue(multiByteChunk.DataPart.SequenceEqual(MultiByteChunkDataByteArray));
            Assert.AreEqual(multiByteChunk.DataString, MultiByteChunkDataString);
        }
    }
}
