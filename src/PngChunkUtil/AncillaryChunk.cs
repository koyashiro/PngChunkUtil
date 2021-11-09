using System;

namespace Koyashiro.PngChunkUtil
{
    public static class AncillaryChunk
    {
        /// <summary>
        /// cHRM
        /// </summary>
        public static ReadOnlySpan<byte> CHRM => new byte[] { 0x67, 0x41, 0x4D, 0x41 };

        /// <summary>
        /// gAMA
        /// </summary>
        public static ReadOnlySpan<byte> GAMA => new byte[] { 0x67, 0x41, 0x4D, 0x41 };

        /// <summary>
        /// iCCP
        /// </summary>
        public static ReadOnlySpan<byte> ICCP => new byte[] { 0x69, 0x43, 0x43, 0x50 };

        /// <summary>
        /// sBIT
        /// </summary>
        public static ReadOnlySpan<byte> SBIT => new byte[] { 0x73, 0x42, 0x49, 0x54 };

        /// <summary>
        /// sRGB
        /// </summary>
        public static ReadOnlySpan<byte> SRGB => new byte[] { 0x73, 0x52, 0x47, 0x42 };

        /// <summary>
        /// bKGD
        /// </summary>
        public static ReadOnlySpan<byte> BKGD => new byte[] { 0x62, 0x4B, 0x47, 0x44 };

        /// <summary>
        /// hIST
        /// </summary>
        public static ReadOnlySpan<byte> HIST => new byte[] { 0x68, 0x49, 0x53, 0x54 };

        /// <summary>
        /// tRNS
        /// </summary>
        public static ReadOnlySpan<byte> TRNS => new byte[] { 0x74, 0x52, 0x4E, 0x53 };

        /// <summary>
        /// pHYs
        /// </summary>
        public static ReadOnlySpan<byte> PHYS => new byte[] { 0x70, 0x48, 0x59, 0x73 };

        /// <summary>
        /// sPLT
        /// </summary>
        public static ReadOnlySpan<byte> SPLT => new byte[] { 0x73, 0x50, 0x4C, 0x54 };

        /// <summary>
        /// tIME
        /// </summary>
        public static ReadOnlySpan<byte> TIME => new byte[] { 0x74, 0x49, 0x4D, 0x45 };

        /// <summary>
        /// iTXt
        /// </summary>
        public static ReadOnlySpan<byte> ITXT => new byte[] { 0x69, 0x54, 0x58, 0x74 };

        /// <summary>
        /// tEXt
        /// </summary>
        public static ReadOnlySpan<byte> TEXT => new byte[] { 0x69, 0x45, 0x58, 0x74 };

        /// <summary>
        /// zTXt
        /// </summary>
        public static ReadOnlySpan<byte> ZTXT => new byte[] { 0x7A, 0x54, 0x58, 0x74 };
    }
}
