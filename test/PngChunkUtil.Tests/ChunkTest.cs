using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace KoyashiroKohaku.PngChunkUtil.Tests
{
    [TestClass]
    public class ChunkTest
    {
        private static IEnumerable<object[]> InvalidChunkValues => new object[][]
        {
            new object[] { Array.Empty<byte>() },
            new object[] { new byte[1] },
            new object[] { new byte[2] },
            new object[] { new byte[3] },
            new object[] { new byte[4] }
        };
        private static IEnumerable<object[]> ValidChunkValues => new object[][]
        {
            new object[]
            {
                new byte[12]
                {
                    0x00, 0x00, 0x00, 0x00,
                    (byte)'I', (byte)'H', (byte)'D', (byte)'R',
                    0xa8, 0xa1, 0xae, 0x0a
                }
            },
            new object[]
            {
                new byte[12]
                {
                    0x00, 0x00, 0x00, 0x00,
                    (byte)'I', (byte)'E', (byte)'N', (byte)'D',
                    0xae, 0x42, 0x60, 0x82
                }
            }
        };

        [TestMethod]
        [TestCategory("Ctor")]
        public void Ctor_InputIsNull_ThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Chunk(null));
        }

        [TestMethod]
        [DynamicData(nameof(InvalidChunkValues))]
        [TestCategory("Ctor")]
        public void Ctor_InputIsInvalid_ThrowArgumentException(byte[] buffer)
        {
            Assert.ThrowsException<ArgumentException>(() => new Chunk(buffer));
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkValues))]
        [TestCategory("Ctor")]
        public void Ctor_InputIsValid_ReturnChunk(byte[] buffer)
        {
            new Chunk(buffer);
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkValues))]
        [TestCategory("Value")]
        public void Value_ReturnChunkValue(byte[] buffer)
        {
            var chunk = new Chunk(buffer);
            CollectionAssert.AreEqual(buffer, chunk.Value.ToArray());
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkValues))]
        [TestCategory("LengthPart")]
        public void LengthPart_ReturnLengthPartValue(byte[] buffer)
        {
            var chunk = new Chunk(buffer);
            CollectionAssert.AreEqual(buffer[..4], chunk.LengthPart.ToArray());
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkValues))]
        [TestCategory("DataLength")]
        public void DataLength_ReturnDataLength(byte[] buffer)
        {
            var chunk = new Chunk(buffer);
            Assert.AreEqual(BinaryPrimitives.ReadInt32BigEndian(buffer[..4]), chunk.DataLength);
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkValues))]
        [TestCategory("TypePart")]
        public void TypePart_ReturnTypePartValue(byte[] buffer)
        {
            var chunk = new Chunk(buffer);
            CollectionAssert.AreEqual(buffer[4..8], chunk.TypePart.ToArray());
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkValues))]
        [TestCategory("TypeString")]
        public void TypeString_ReturnTypeString(byte[] buffer)
        {
            var chunk = new Chunk(buffer);
            Assert.AreEqual(Encoding.UTF8.GetString(buffer[4..8]), chunk.TypeString);
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkValues))]
        [TestCategory("DataPart")]
        public void DataPart_ReturnDataPartValue(byte[] buffer)
        {
            var chunk = new Chunk(buffer);
            CollectionAssert.AreEqual(buffer[8..^4], chunk.DataPart.ToArray());
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkValues))]
        [TestCategory("DataString")]
        public void DataString_ReturnTypeString(byte[] buffer)
        {
            var chunk = new Chunk(buffer);
            Assert.AreEqual(Encoding.UTF8.GetString(buffer[8..^4]), chunk.DataString);
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkValues))]
        [TestCategory("CrcPart")]
        public void CrcPart_ReturnCrcPartValue(byte[] buffer)
        {
            var chunk = new Chunk(buffer);
            CollectionAssert.AreEqual(buffer[^4..], chunk.CrcPart.ToArray());
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkValues))]
        [TestCategory("Crc")]
        public void Crc_ReturnCrc(byte[] buffer)
        {
            var chunk = new Chunk(buffer);
            Assert.AreEqual(BinaryPrimitives.ReadUInt32BigEndian(buffer[^4..]), chunk.Crc);
        }
    }
}
