using System;

namespace KoyashiroKohaku.PngMetaDataUtil
{
    public static class CriticalChunk
    {
        /// <summary>
        /// IHDR
        /// </summary>
        public static ReadOnlySpan<byte> IHDR => new byte[] { 0x49, 0x48, 0x44, 0x52 };

        /// <summary>
        /// PLTE
        /// </summary>
        public static ReadOnlySpan<byte> PLTE => new byte[] { 0x50, 0x4C, 0x54, 0x45 };

        /// <summary>
        /// IDAT
        /// </summary>
        public static ReadOnlySpan<byte> IDAT => new byte[] { 0x49, 0x44, 0x41, 0x54 };

        /// <summary>
        /// IEND
        /// </summary>
        public static ReadOnlySpan<byte> IEND => new byte[] { 0x49, 0x45, 0x4E, 0x44 };
    }
}
