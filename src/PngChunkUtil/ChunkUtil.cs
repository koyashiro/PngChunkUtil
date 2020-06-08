using System;
using System.Linq;

namespace KoyashiroKohaku.PngChunkUtil
{
    public static class ChunkUtil
    {
        public static bool IsCriticalChunk(ReadOnlySpan<byte> chunkType)
        {
            return chunkType.SequenceEqual(CriticalChunk.IHDR)
                || chunkType.SequenceEqual(CriticalChunk.PLTE)
                || chunkType.SequenceEqual(CriticalChunk.IDAT)
                || chunkType.SequenceEqual(CriticalChunk.IEND);
        }

        public static bool IsAncillaryChunk(ReadOnlySpan<byte> chunkType)
        {
            return chunkType.SequenceEqual(AncillaryChunk.cHRM)
                || chunkType.SequenceEqual(AncillaryChunk.gAMA)
                || chunkType.SequenceEqual(AncillaryChunk.iCCP)
                || chunkType.SequenceEqual(AncillaryChunk.sBIT)
                || chunkType.SequenceEqual(AncillaryChunk.sRGB)
                || chunkType.SequenceEqual(AncillaryChunk.bKGD)
                || chunkType.SequenceEqual(AncillaryChunk.hIST)
                || chunkType.SequenceEqual(AncillaryChunk.tRNS)
                || chunkType.SequenceEqual(AncillaryChunk.pHYs)
                || chunkType.SequenceEqual(AncillaryChunk.tIME)
                || chunkType.SequenceEqual(AncillaryChunk.iTXt)
                || chunkType.SequenceEqual(AncillaryChunk.tEXt)
                || chunkType.SequenceEqual(AncillaryChunk.zTXt);
        }

        public static bool IsAdditionalChunk(ReadOnlySpan<byte> chunkType)
        {
            return !IsCriticalChunk(chunkType) && !IsAncillaryChunk(chunkType);
        }
    }
}
