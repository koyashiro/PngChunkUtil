using System;

namespace KoyashiroKohaku.PngChunkUtil
{
    public static class PngUtil
    {
        /// <summary>
        /// PNG画像のシグネチャ
        /// </summary>
        public static ReadOnlySpan<byte> Signature => new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        /// <summary>
        /// PNG画像かどうかをチェックします。
        /// </summary>
        /// <param name="source">PNG画像のSpan構造体</param>
        /// <returns></returns>
        public static bool IsPng(ReadOnlySpan<byte> source)
        {
            if (source == null)
            {
                return false;
            }

            if (source.Length < 8)
            {
                return false;
            }

            return source.Slice(0, 8).SequenceEqual(Signature);
        }
    }
}
