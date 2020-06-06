using System;
using System.Text;

namespace KoyashiroKohaku.PngMetaDataUtil
{
    /// <summary>
    /// ChunkType
    /// </summary>
    public class ChunkType
    {
        /// <summary>
        /// ChunkType
        /// </summary>
        private readonly byte[] _chunkType;

        internal ChunkType()
        {
            _chunkType = new byte[4];
        }

        internal ChunkType(ReadOnlySpan<byte> type)
        {
            if (type == null)
            {
                throw new ArgumentNullException();
            }

            if (type.Length != 4)
            {
                throw new ArgumentException();
            }

            _chunkType = type.ToArray();
        }

        internal ChunkType(ReadOnlySpan<char> type)
        {
            if (type == null)
            {
                throw new ArgumentNullException();
            }

            ReadOnlySpan<byte> span = Encoding.UTF8.GetBytes(type.ToArray());

            if (span.Length != 4)
            {
                throw new ArgumentException();
            }

            _chunkType = span.ToArray();
        }

        /// <summary>
        /// <see cref="PngMetaDataParser"/>のSpan構造体を返却します。
        /// </summary>
        public Span<byte> Value => _chunkType;
        
        public void SetValue(ReadOnlySpan<byte> value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            if (value.Length != 4)
            {
                throw new ArgumentException();
            }

            for (int i = 0; i < value.Length; i++)
            {
                Value[i] = value[i];
            }
        }

        public void SetValue (ReadOnlySpan<char> value)
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

            return encoding.GetString(_chunkType);
        }
    }
}
