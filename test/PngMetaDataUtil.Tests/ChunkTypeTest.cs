using Microsoft.VisualStudio.TestTools.UnitTesting;
using KoyashiroKohaku.PngMetaDataUtil;
using System;
using System.Linq;
using System.IO;

namespace KoyashiroKohaku.PngMetaDataUtil.Tests
{
    [TestClass]
    public class ChunkTypeTest
    {
        internal static byte[] IhdrChunkTypeByteArray => new byte[] { 0x49, 0x48, 0x44, 0x52 };
        internal static string IhdrChunkTypeString => "IHDR";
        internal static byte[] IdatChunkTypeByteArray => new byte[] { 0x49, 0x44, 0x41, 0x54 };
        internal static string IdatChunkTypeString => "IDAT";

        [TestMethod]
        public void Constructor_WithNoArgmentTest()
        {
            var chunkType = new ChunkType();

            Assert.IsTrue(chunkType.Value.SequenceEqual(new byte[4]));
        }

        [TestMethod]
        public void Constructor_WithByteSpanTest()
        {
            var chunkType = new ChunkType(IhdrChunkTypeByteArray);

            Assert.IsTrue(chunkType.Value.SequenceEqual(IhdrChunkTypeByteArray));
            Assert.AreEqual(IhdrChunkTypeString, chunkType.ToString());
        }

        [TestMethod]
        public void Constructor_WithCharSpanTest()
        {
            var chunkType = new ChunkType(IhdrChunkTypeString);

            Assert.IsTrue(chunkType.Value.SequenceEqual(IhdrChunkTypeByteArray));
            Assert.AreEqual(IhdrChunkTypeString, chunkType.ToString());
        }

        [TestMethod]
        public void Constructor_SetValueWithByteSpan()
        {
            var chunkType = new ChunkType(IhdrChunkTypeByteArray);

            chunkType.SetValue(IdatChunkTypeByteArray);

            Assert.IsTrue(chunkType.Value.SequenceEqual(IdatChunkTypeByteArray));
            Assert.AreEqual(IdatChunkTypeString, chunkType.ToString());
        }

        [TestMethod]
        public void Constructor_SetValueWithCharSpan()
        {
            var chunkType = new ChunkType(IhdrChunkTypeByteArray);

            chunkType.SetValue(IdatChunkTypeString);

            Assert.IsTrue(chunkType.Value.SequenceEqual(IdatChunkTypeByteArray));
            Assert.AreEqual(IdatChunkTypeString, chunkType.ToString());
        }
    }
}
