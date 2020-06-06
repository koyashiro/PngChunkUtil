using System;
using System.Text;

namespace KoyashiroKohaku.PngMetaDataTool
{
    /// <summary>
    /// ChunkData
    /// </summary>
    public class ChunkData
    {
        /// <summary>
        /// ChunkData
        /// </summary>
        private readonly byte[] _chunkData;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="data"></param>
        public ChunkData(Span<byte> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException($"argument error. argument: '{nameof(data)}' is null.");
            }

            _chunkData = data.ToArray();
        }

        /// <summary>
        /// <see cref="PngMetaDataParser"/>のSpan構造体を返却します。
        /// </summary>
        public Span<byte> Value => _chunkData.AsSpan();

        /// <summary>
        /// ChunkDataのバイト長
        /// </summary>
        public int Length => _chunkData.Length;

        public override string ToString() => ToString(Encoding.UTF8);

        public string ToString(Encoding encoding)
        {
            if (encoding is null)
            {
                encoding = Encoding.UTF8;
            }

            return encoding.GetString(_chunkData);
        }
    }
}
