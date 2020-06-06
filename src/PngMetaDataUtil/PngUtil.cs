using System;
using System.Collections.Generic;
using System.Text;

namespace KoyashiroKohaku.PngMetaDataUtil
{
    public static class PngUtil
    {
        /// <summary>
        /// PNG画像のシグネチャ
        /// </summary>
        public static ReadOnlySpan<byte> Signature => new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

    }
}
