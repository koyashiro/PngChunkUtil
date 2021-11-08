using System;

namespace KoyashiroKohaku.PngChunkUtil
{
    public static class Png
    {
        public static ReadOnlySpan<byte> Signature => new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
    }
}
