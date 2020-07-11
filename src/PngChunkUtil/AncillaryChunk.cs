using System;

namespace KoyashiroKohaku.PngChunkUtil
{
    public static class AncillaryChunk
    {
        /// <summary>
        /// cHRM
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名スタイル")]
        public static ReadOnlySpan<byte> cHRM => new byte[] { 0x67, 0x41, 0x4D, 0x41 };

        /// <summary>
        /// gAMA
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名スタイル")]
        public static ReadOnlySpan<byte> gAMA => new byte[] { 0x67, 0x41, 0x4D, 0x41 };

        /// <summary>
        /// iCCP
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名スタイル")]
        public static ReadOnlySpan<byte> iCCP => new byte[] { 0x69, 0x43, 0x43, 0x50 };

        /// <summary>
        /// sBIT
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名スタイル")]
        public static ReadOnlySpan<byte> sBIT => new byte[] { 0x73, 0x42, 0x49, 0x54 };

        /// <summary>
        /// sRGB
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名スタイル")]
        public static ReadOnlySpan<byte> sRGB => new byte[] { 0x73, 0x52, 0x47, 0x42 };

        /// <summary>
        /// bKGD
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名スタイル")]
        public static ReadOnlySpan<byte> bKGD => new byte[] { 0x62, 0x4B, 0x47, 0x44 };

        /// <summary>
        /// hIST
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名スタイル")]
        public static ReadOnlySpan<byte> hIST => new byte[] { 0x68, 0x49, 0x53, 0x54 };

        /// <summary>
        /// tRNS
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名スタイル")]
        public static ReadOnlySpan<byte> tRNS => new byte[] { 0x74, 0x52, 0x4E, 0x53 };

        /// <summary>
        /// pHYs
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名スタイル")]
        public static ReadOnlySpan<byte> pHYs => new byte[] { 0x70, 0x48, 0x59, 0x73 };

        /// <summary>
        /// sPLT
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名スタイル")]
        public static ReadOnlySpan<byte> sPLT => new byte[] { 0x73, 0x50, 0x4C, 0x54 };

        /// <summary>
        /// tIME
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名スタイル")]
        public static ReadOnlySpan<byte> tIME => new byte[] { 0x74, 0x49, 0x4D, 0x45 };

        /// <summary>
        /// iTXt
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名スタイル")]
        public static ReadOnlySpan<byte> iTXt => new byte[] { 0x69, 0x54, 0x58, 0x74 };

        /// <summary>
        /// tEXt
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名スタイル")]
        public static ReadOnlySpan<byte> tEXt => new byte[] { 0x69, 0x45, 0x58, 0x74 };

        /// <summary>
        /// zTXt
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名スタイル")]
        public static ReadOnlySpan<byte> zTXt => new byte[] { 0x7A, 0x54, 0x58, 0x74 };
    }
}
