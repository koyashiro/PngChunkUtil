using System;
using System.Text;

namespace KoyashiroKohaku.PngMetaDataUtil
{
    /// <summary>
    /// ChunkData
    /// </summary>
    public class ChunkData
    {
        /// <summary>
        /// ChunkData
        /// </summary>
        private byte[] _chunkData;

        internal ChunkData()
        {
            _chunkData = new byte[8];
        }

        internal ChunkData(ReadOnlySpan<byte> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException();
            }

            _chunkData = data.ToArray();
        }

        internal ChunkData(ReadOnlySpan<char> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException();
            }

            _chunkData = Encoding.UTF8.GetBytes(data.ToArray());
        }

        /// <summary>
        /// <see cref="ChunkReader"/>のSpan構造体を返却します。
        /// </summary>
        public Span<byte> Value => _chunkData;

        /// <summary>
        /// ChunkDataのバイト長
        /// </summary>
        public int Length => _chunkData.Length;

        public void SetValue(ReadOnlySpan<byte> value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            if (_chunkData.Length == value.Length)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    Value[i] = value[i];
                }
            }
            else
            {
                _chunkData = value.ToArray();
            }
        }

        public void SetValue(ReadOnlySpan<char> value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            SetValue(Encoding.UTF8.GetBytes(value.ToArray()));
        }

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
