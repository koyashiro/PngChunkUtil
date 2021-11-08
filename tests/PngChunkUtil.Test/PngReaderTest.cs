using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KoyashiroKohaku.PngChunkUtil.Test
{
    [TestClass]
    public class PngReaderTest
    {
        private static readonly byte[] _invalidImage = File.ReadAllBytes(@"Assets/invalid.jpg");
        private static readonly byte[] _validImage = File.ReadAllBytes(@"Assets/valid.png");
        private static readonly byte[] _almostValidImage = File.ReadAllBytes(@"Assets/almost_valid.png");

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
            new object[] { _validImage },
            new object[] { _almostValidImage }
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
            new object[] { PngReader.Parse(_validImage).ToArray() },
            new object[] { PngReader.Parse(_almostValidImage).ToArray() },
        };

        [TestMethod]
        [TestCategory(nameof(PngReader.Parse))]
        public void Parse_InputIsNull_ThrowArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => PngReader.Parse(null));
        }

        [DataTestMethod]
        [DynamicData(nameof(InvalidPngs))]
        [TestCategory(nameof(PngReader.Parse))]
        public void Parse_InputIsInvalid_ThrowArgumentException(byte[] image)
        {
            Assert.ThrowsException<ArgumentException>(() => PngReader.Parse(image));
        }

        [DataTestMethod]
        [DynamicData(nameof(ValidPngs))]
        [TestCategory(nameof(PngReader.Parse))]
        public void Parse_InputIsValid_ReturnChunks(byte[] image)
        {
            var chunks = PngReader.Parse(image).ToArray();

            Assert.IsTrue(chunks.Any());
            Assert.IsTrue(chunks.All(c => c.IsValid()));
            Assert.AreEqual("IHDR", chunks.First().ChunkType());
            Assert.IsTrue(chunks.Any(c => c.ChunkType() == "IDAT"));
            Assert.AreEqual("IEND", chunks.Last().ChunkType());
        }

        [TestMethod]
        [TestCategory(nameof(PngReader.TryParse))]
        public void TryParse_InputIsNull_ReturnFalseAndDefault()
        {
            Assert.IsFalse(PngReader.TryParse(null, out var chunks));
            Assert.AreEqual(default, chunks);
        }

        [TestMethod]
        [DynamicData(nameof(InvalidPngs))]
        [TestCategory(nameof(PngReader.TryParse))]
        public void TryParse_InputIsInvalid_ReturnFalseAndDefault(byte[] image)
        {
            Assert.IsFalse(PngReader.TryParse(image, out var chunks));
            Assert.AreEqual(default, chunks);
        }

        [TestMethod]
        [DynamicData(nameof(ValidPngs))]
        [TestCategory(nameof(PngReader.TryParse))]
        public void TryParse_InputIsValid_ReturnTrunAndChunks(byte[] image)
        {
            Assert.IsTrue(PngReader.TryParse(image, out var chunks));
            Assert.IsTrue(chunks.Any());
            Assert.IsTrue(chunks.All(c => c.IsValid()));
            Assert.AreEqual("IHDR", chunks.First().ChunkType());
            Assert.IsTrue(chunks.Any(c => c.ChunkType() == "IDAT"));
            Assert.AreEqual("IEND", chunks.Last().ChunkType());
        }

    }
}
