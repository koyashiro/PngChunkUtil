using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Koyashiro.PngChunkUtil.Test
{
    [TestClass]
    public class PngWriterTest
    {
        private static readonly byte[] _invalidImage = File.ReadAllBytes(@"Assets/invalid.jpg");
        private static readonly byte[] _validImage = File.ReadAllBytes(@"Assets/valid.png");

        private static IEnumerable<object[]> InvalidPngs => new object[][]
        {
            new object[] { Array.Empty<byte>() },
            new object[] { new byte[1] },
            new object[] { new byte[2] },
            new object[] { new byte[3] },
            new object[] { new byte[4] },
            new object[] { new byte[5] },
            new object[] { new byte[6] },
            new object[] { new byte[7] },
            new object[] { _invalidImage }
        };

        private static IEnumerable<object[]> ValidPngs => new object[][]
        {
            new object[] { _validImage }
        };

        private static IEnumerable<object[]> InvalidChunks => new object[][]
        {
            new object[] { Array.Empty<Chunk>() },
            new object[] { new Chunk[1] },
            new object[] { new Chunk[2] },
            new object[] { new Chunk[3] },
            new object[] { new Chunk[4] }
        };

        private static IEnumerable<object[]> ValidChunks => new object[][]
        {
            new object[] { PngReader.ReadBytes(_validImage).ToArray() }
        };

        [DataTestMethod]
        [TestCategory(nameof(PngWriter.WriteBytes))]
        public void WriteBytes_InputIsNull_ThrowArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => PngWriter.WriteBytes(null));
        }

        [DataTestMethod]
        [DynamicData(nameof(InvalidChunks))]
        [TestCategory(nameof(PngWriter.WriteBytes))]
        public void WriteBytes_InputIsInvalid_ThrowArgumentException(Chunk[] chunks)
        {
            Assert.ThrowsException<ArgumentException>(() => PngWriter.WriteBytes(chunks));
        }

        [DataTestMethod]
        [DynamicData(nameof(ValidChunks))]
        [TestCategory(nameof(PngWriter.WriteBytes))]
        public void WriteBytes_InputIsValid_ReturnPngArray(Chunk[] chunks)
        {
            var png = PngWriter.WriteBytes(chunks);
            var resplittedChunks = PngReader.ReadBytes(png).ToArray();
            Assert.AreEqual(chunks.Length, resplittedChunks.Length);

            for (int i = 0; i < chunks.Length; i++)
            {
                var chunk = chunks[i];
                var resplittedChunk = resplittedChunks[i];

                Assert.IsTrue(chunk.Bytes.SequenceEqual(resplittedChunk.Bytes));
            }
        }

        [DataTestMethod]
        [TestCategory(nameof(PngWriter.WriteBytes))]
        public void TryWriteBytes_InputIsNull_ThrowArgumentException()
        {
            Assert.IsFalse(PngWriter.TryWriteBytes(null, out var png));
            Assert.AreEqual(default, png);
        }

        [DataTestMethod]
        [DynamicData(nameof(InvalidChunks))]
        [TestCategory(nameof(PngWriter.WriteBytes))]
        public void TryWriteBytes_InputIsInvalid_ThrowArgumentException(Chunk[] chunks)
        {
            Assert.IsFalse(PngWriter.TryWriteBytes(chunks, out var png));
            Assert.AreEqual(default, png);
        }

        [DataTestMethod]
        [DynamicData(nameof(ValidChunks))]
        [TestCategory(nameof(PngWriter.WriteBytes))]
        public void TryWriteBytes_InputIsValid_ReturnPngArray(Chunk[] chunks)
        {
            Assert.IsTrue(PngWriter.TryWriteBytes(chunks, out var png));
            var resplittedChunks = PngReader.ReadBytes(png).ToArray();
            Assert.AreEqual(chunks.Length, resplittedChunks.Length);

            for (int i = 0; i < chunks.Length; i++)
            {
                var chunk = chunks[i];
                var resplittedChunk = resplittedChunks[i];

                Assert.IsTrue(chunk.Bytes.SequenceEqual(resplittedChunk.Bytes));
            }
        }
    }
}
