using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KoyashiroKohaku.PngChunkUtil.Tests
{
    [TestClass]
    public class ChunkTest
    {
        private static IEnumerable<object[]> InvalidChunkBytes => new object[][]
        {
            new object[] { Array.Empty<byte>() },
            new object[] { new byte[1] },
            new object[] { new byte[2] },
            new object[] { new byte[3] },
            new object[] { new byte[4] }
        };

        private static IEnumerable<object[]> ValidChunkBytes => new object[][]
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

        private static IEnumerable<object[]> InvalidChunkBytesParams => new object[][]
        {
            new object[]
            {
                new byte[1]
                {
                    (byte)'A'
                },
                new byte[4]
                {
                    0xa8, 0xa1, 0xae, 0x0a
                }
            },
            new object[]
            {
                new byte[2]
                {
                    (byte)'A', (byte)'B'
                },
                new byte[4]
                {
                    0xa8, 0xa1, 0xae, 0x0a
                }
            },
            new object[]
            {
                new byte[3]
                {
                    (byte)'A', (byte)'B', (byte)'C'
                },
                new byte[4]
                {
                    0xa8, 0xa1, 0xae, 0x0a
                }
            },
            new object[]
            {
                new byte[5]
                {
                    (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E'
                },
                new byte[4]
                {
                    0xa8, 0xa1, 0xae, 0x0a
                }
            }
        };

        private static IEnumerable<object[]> InvalidChunkStringBytesParams => new object[][]
        {
            new object[]
            {
                "A",
                new byte[4]
                {
                    0xa8, 0xa1, 0xae, 0x0a
                }
            },
            new object[]
            {
                "AB",
                new byte[4]
                {
                    0xa8, 0xa1, 0xae, 0x0a
                }
            },
            new object[]
            {
                "ABC",
                new byte[4]
                {
                    0xa8, 0xa1, 0xae, 0x0a
                }
            },
            new object[]
            {
                "ABCDE",
                new byte[4]
                {
                    0xa8, 0xa1, 0xae, 0x0a
                }
            }
        };

        private static IEnumerable<object[]> InvalidChunkStringParams => new object[][]
        {
            new object[]
            {
                "A",
                "XYZ"
            },
            new object[]
            {
                "AB",
                "XYZ"
            },
            new object[]
            {
                "ABC",
                "XYZ"
            },
            new object[]
            {
                "ABCDE",
                "XYZ"
            }
        };

        private static IEnumerable<object[]> ValidChunkBytesParams => new object[][]
        {
            new object[]
            {
                new byte[4]
                {
                    (byte)'I', (byte)'H', (byte)'D', (byte)'R'
                },
                new byte[4]
                {
                    0xa8, 0xa1, 0xae, 0x0a
                }
            },
            new object[]
            {
                new byte[4]
                {
                    (byte)'I', (byte)'E', (byte)'N', (byte)'D'
                },
                new byte[4]
                {
                    0xae, 0x42, 0x60, 0x82
                }
            }
        };

        private static IEnumerable<object[]> ValidChunkStringBytesParams => new object[][]
        {
            new object[]
            {
                "IHDR",
                new byte[4]
                {
                    0xa8, 0xa1, 0xae, 0x0a
                }
            },
            new object[]
            {
                "IEND",
                new byte[4]
                {
                    0xa8, 0xa1, 0xae, 0x0a
                }
            }
        };

        private static IEnumerable<object[]> ValidChunkStringParams => new object[][]
        {
            new object[]
            {
                "IHDR",
                "XYZ"
            },
            new object[]
            {
                "IEND",
                "XYZ"
            }
        };

        [TestMethod]
        [TestCategory("Ctor")]
        public void Ctor_NoArgument_ReturnDefault()
        {
            Assert.AreEqual(default(Chunk), new Chunk());
        }

        [TestMethod]
        [DynamicData(nameof(InvalidChunkBytes))]
        [TestCategory(nameof(Chunk.Parse))]
        public void Parse_InvalidChunkBytes_ThrowArgumentException(byte[] buffer)
        {
            Assert.ThrowsException<ArgumentException>(() => Chunk.Parse(buffer));
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkBytes))]
        [TestCategory(nameof(Chunk.Parse))]
        public void Parse_ValidChunkBytes_ReturnChunk(byte[] buffer)
        {
            var chunk = Chunk.Parse(buffer);
            Assert.AreNotEqual(default(Chunk), chunk);
        }

        [TestMethod]
        [DynamicData(nameof(InvalidChunkBytes))]
        [TestCategory(nameof(Chunk.TryParse))]
        public void TryParse_InvalidChunkBytes_ReturnFalseAndDefault(byte[] buffer)
        {
            Assert.IsFalse(Chunk.TryParse(buffer, out var chunk));
            Assert.AreEqual(default(Chunk), chunk);
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkBytes))]
        [TestCategory(nameof(Chunk.TryParse))]
        public void TryParse_ValidChunkBytes_ReturnTrueAndChunk(byte[] buffer)
        {
            Assert.IsTrue(Chunk.TryParse(buffer, out var chunk));
            Assert.AreNotEqual(default(Chunk), chunk);
        }

        [TestMethod]
        [DynamicData(nameof(InvalidChunkBytesParams))]
        [TestCategory(nameof(Chunk.Create))]
        public void Create_InvalidChunkBytesParams_ThrowArgumentException(byte[] chunkType, byte[] chunkData)
        {
            Assert.ThrowsException<ArgumentException>(() => Chunk.Create(chunkType, chunkData));
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkBytesParams))]
        [TestCategory(nameof(Chunk.Create))]
        public void Create_ValidChunkBytesParams_ReturnChunk(byte[] chunkType, byte[] chunkData)
        {
            var chunk = Chunk.Create(chunkType, chunkData);
            Assert.AreNotEqual(default(Chunk), chunk);
            Assert.IsTrue(chunk.IsValid());
        }

        [TestMethod]
        [DynamicData(nameof(InvalidChunkBytesParams))]
        [TestCategory(nameof(Chunk.TryCreate))]
        public void TryCreate_InvalidChunkBytesParams_ThrowArgumentException(byte[] chunkType, byte[] chunkData)
        {
            Assert.IsFalse(Chunk.TryCreate(chunkType, chunkData, out var chunk));
            Assert.AreEqual(default(Chunk), chunk);
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkBytesParams))]
        [TestCategory(nameof(Chunk.TryCreate))]
        public void TryCreate_ValidChunkBytesParams_ReturnChunk(byte[] chunkType, byte[] chunkData)
        {
            Assert.IsTrue(Chunk.TryCreate(chunkType, chunkData, out var chunk));
            Assert.AreNotEqual(default(Chunk), chunk);
            Assert.IsTrue(chunk.IsValid());
        }

        [TestMethod]
        [DynamicData(nameof(InvalidChunkStringBytesParams))]
        [TestCategory(nameof(Chunk.Create))]
        public void Create_InvalidChunkStringBytesParams_ThrowArgumentException(string chunkType, byte[] chunkData)
        {
            Assert.ThrowsException<ArgumentException>(() => Chunk.Create(chunkType, chunkData));
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkStringBytesParams))]
        [TestCategory(nameof(Chunk.Create))]
        public void Create_ValidChunkStringBytesParams_ReturnChunk(string chunkType, byte[] chunkData)
        {
            var chunk = Chunk.Create(chunkType, chunkData);
            Assert.AreNotEqual(default(Chunk), chunk);
            Assert.IsTrue(chunk.IsValid());
        }

        [TestMethod]
        [DynamicData(nameof(InvalidChunkStringBytesParams))]
        [TestCategory(nameof(Chunk.TryCreate))]
        public void TryCreate_InvalidChunkStringBytesParams_ThrowArgumentException(string chunkType, byte[] chunkData)
        {
            Assert.IsFalse(Chunk.TryCreate(chunkType, chunkData, out var chunk));
            Assert.AreEqual(default(Chunk), chunk);
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkStringBytesParams))]
        [TestCategory(nameof(Chunk.TryCreate))]
        public void TryCreate_ValidChunkStringBytesParams_ReturnChunk(string chunkType, byte[] chunkData)
        {
            Assert.IsTrue(Chunk.TryCreate(chunkType, chunkData, out var chunk));
            Assert.AreNotEqual(default(Chunk), chunk);
            Assert.IsTrue(chunk.IsValid());
        }

        [TestMethod]
        [DynamicData(nameof(InvalidChunkStringParams))]
        [TestCategory(nameof(Chunk.Create))]
        public void Create_InvalidChunkStringParams_ThrowArgumentException(string chunkType, string chunkData)
        {
            Assert.ThrowsException<ArgumentException>(() => Chunk.Create(chunkType, chunkData));
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkStringParams))]
        [TestCategory(nameof(Chunk.Create))]
        public void Create_ValidChunkStringParams_ReturnChunk(string chunkType, string chunkData)
        {
            var chunk = Chunk.Create(chunkType, chunkData);
            Assert.AreNotEqual(default(Chunk), chunk);
            Assert.IsTrue(chunk.IsValid());
        }

        [TestMethod]
        [DynamicData(nameof(InvalidChunkStringParams))]
        [TestCategory(nameof(Chunk.TryCreate))]
        public void TryCreate_InvalidChunkStringParams_ThrowArgumentException(string chunkType, string chunkData)
        {
            Assert.IsFalse(Chunk.TryCreate(chunkType, chunkData, out var chunk));
            Assert.AreEqual(default(Chunk), chunk);
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkStringParams))]
        [TestCategory(nameof(Chunk.TryCreate))]
        public void TryCreate_ValidChunkStringParams_ReturnChunk(string chunkType, string chunkData)
        {
            Assert.IsTrue(Chunk.TryCreate(chunkType, chunkData, out var chunk));
            Assert.AreNotEqual(default(Chunk), chunk);
            Assert.IsTrue(chunk.IsValid());
        }

        [TestMethod]
        [TestCategory(nameof(Chunk.Bytes))]
        public void Bytes_Default_ReturnEmpty()
        {
            var chunk = default(Chunk);
            CollectionAssert.AreEqual(Array.Empty<byte>(), chunk.Bytes.ToArray());
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkBytes))]
        [TestCategory(nameof(Chunk.Bytes))]
        public void Bytes_ValidChunkBytes_ReturnChunkBytes(byte[] buffer)
        {
            var chunk = Chunk.Parse(buffer);
            CollectionAssert.AreEqual(buffer, chunk.Bytes.ToArray());
        }

        [TestMethod]
        [TestCategory(nameof(Chunk.LengthBytes))]
        public void LengthBytes_Default_ReturnEmpty()
        {
            var chunk = default(Chunk);
            CollectionAssert.AreEqual(Array.Empty<byte>(), chunk.LengthBytes.ToArray());
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkBytes))]
        [TestCategory(nameof(Chunk.LengthBytes))]
        public void LengthBytes_ValidChunkBytes_ReturnLengthBytes(byte[] buffer)
        {
            var chunk = Chunk.Parse(buffer);
            CollectionAssert.AreEqual(buffer[..4], chunk.LengthBytes.ToArray());
        }


        [TestMethod]
        [TestCategory(nameof(Chunk.ChunkTypeBytes))]
        public void ChunkTypeBytes_Default_ReturnEmpty()
        {
            var chunk = default(Chunk);
            CollectionAssert.AreEqual(Array.Empty<byte>(), chunk.ChunkTypeBytes.ToArray());
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkBytes))]
        [TestCategory(nameof(Chunk.ChunkTypeBytes))]
        public void ChunkTypeBytes_ValidChunkBytes_ReturnChunkTypeBytes(byte[] buffer)
        {
            var chunk = Chunk.Parse(buffer);
            CollectionAssert.AreEqual(buffer[4..8], chunk.ChunkTypeBytes.ToArray());
        }

        [TestMethod]
        [TestCategory(nameof(Chunk.ChunkDataBytes))]
        public void ChunkDataBytes_Default_ReturnEmpty()
        {
            var chunk = default(Chunk);
            CollectionAssert.AreEqual(Array.Empty<byte>(), chunk.ChunkDataBytes.ToArray());
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkBytes))]
        [TestCategory(nameof(Chunk.ChunkDataBytes))]
        public void ChunkDataBytes_ValidChunkBytes_ReturnChunkDataBytes(byte[] buffer)
        {
            var chunk = Chunk.Parse(buffer);
            CollectionAssert.AreEqual(buffer[8..^4], chunk.ChunkDataBytes.ToArray());
        }

        [TestMethod]
        [TestCategory(nameof(Chunk.CrcBytes))]
        public void CrcBytes_Default_ReturnEmpty()
        {
            var chunk = default(Chunk);
            CollectionAssert.AreEqual(Array.Empty<byte>(), chunk.CrcBytes.ToArray());
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkBytes))]
        [TestCategory(nameof(Chunk.CrcBytes))]
        public void CrcBytes_ValidChunkBytes_ReturnCrcBytes(byte[] buffer)
        {
            var chunk = Chunk.Parse(buffer);
            CollectionAssert.AreEqual(buffer[^4..], chunk.CrcBytes.ToArray());
        }

        [TestMethod]
        [TestCategory(nameof(Chunk.ChunkDataLength))]
        public void ChunkDataLength_Default_ReturnDefault()
        {
            var chunk = default(Chunk);
            Assert.AreEqual(default(int?), chunk.ChunkDataLength);
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkBytes))]
        [TestCategory(nameof(Chunk.ChunkDataLength))]
        public void ChunkDataLength_ValidChunkBytes_ReturnChunkDataLength(byte[] buffer)
        {
            var chunk = Chunk.Parse(buffer);
            Assert.AreEqual(BinaryPrimitives.ReadInt32BigEndian(buffer[..4]), chunk.ChunkDataLength);
        }

        [TestMethod]
        [TestCategory(nameof(Chunk.ChunkType))]
        public void ChunkType_Default_ReturnEmpty()
        {
            var chunk = default(Chunk);
            Assert.AreEqual(default(string), chunk.ChunkType());
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkBytes))]
        [TestCategory(nameof(Chunk.ChunkType))]
        public void ChunkType_ValidChunkBytes_ReturnChunkType(byte[] buffer)
        {
            var chunk = Chunk.Parse(buffer);
            Assert.AreEqual(Encoding.UTF8.GetString(buffer[4..8]), chunk.ChunkType());
        }

        [TestMethod]
        [TestCategory(nameof(Chunk.ChunkData))]
        public void ChunkData_Default_ReturnEmpty()
        {
            var chunk = default(Chunk);
            Assert.AreEqual(default(string), chunk.ChunkData());
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkBytes))]
        [TestCategory(nameof(Chunk.ChunkData))]
        public void ChunkData_ValidChunkBytes_ReturnChunkData(byte[] buffer)
        {
            var chunk = Chunk.Parse(buffer);
            Assert.AreEqual(Encoding.UTF8.GetString(buffer[8..^4]), chunk.ChunkData());
        }

        [TestMethod]
        [TestCategory(nameof(Chunk.ChunkData))]
        public void Crc_Default_ReturnDefault()
        {
            var chunk = default(Chunk);
            Assert.AreEqual(default(uint?), chunk.Crc());
        }

        [TestMethod]
        [DynamicData(nameof(ValidChunkBytes))]
        [TestCategory(nameof(Chunk.ChunkData))]
        public void Crc_ValidChunkBytes_ReturnCrc(byte[] buffer)
        {
            var chunk = Chunk.Parse(buffer);
            Assert.AreEqual(BinaryPrimitives.ReadUInt32BigEndian(buffer[^4..]), chunk.Crc());
        }
    }
}
