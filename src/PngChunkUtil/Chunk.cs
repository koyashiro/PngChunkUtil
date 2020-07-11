using Force.Crc32;
using KoyashiroKohaku.PngChunkUtil.Properties;
using System;
using System.Buffers.Binary;
using System.Text;

namespace KoyashiroKohaku.PngChunkUtil
{
    /// <summary>
    /// Chunk
    /// </summary>
    public class Chunk
    {
        private byte[] _value;

        public Chunk()
        {
            _value = new byte[12];
            UpdateCrc();
        }

        public Chunk(ReadOnlySpan<byte> chunk)
        {
            if (chunk == null)
            {
                throw new ArgumentNullException(nameof(chunk));
            }

            if (!IsValid(chunk))
            {
                throw new ArgumentException(Resources.Chunk_Constructor_Invalid);
            }

            _value = chunk.ToArray();
        }

        public Chunk(ReadOnlySpan<byte> type, ReadOnlySpan<byte> data)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.Length != 4)
            {
                throw new ArgumentException(Resources.Chunk_Constructor_ChunkTypeOutOfRange);
            }

            if (data == null)
            {
                _value = new byte[12];
                UpdateCrc();
                return;
            }

            _value = new byte[12 + data.Length];
            BinaryPrimitives.WriteInt32BigEndian(WritableLengthPart, data.Length);
            type.CopyTo(WritableTypePart);
            data.CopyTo(WritableDataPart);
            UpdateCrc();
        }

        public Chunk(string type, string? data)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.Length != 4)
            {
                throw new ArgumentException(Resources.Chunk_Constructor_ChunkTypeOutOfRange);
            }

            if (data == null)
            {
                _value = new byte[12];
                UpdateCrc();
                return;
            }

            var typeByte = Encoding.UTF8.GetBytes(type);
            var dataByte = string.IsNullOrEmpty(data) ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(data);

            _value = new byte[12 + dataByte.Length];

            BinaryPrimitives.WriteInt32BigEndian(WritableLengthPart, dataByte.Length);
            typeByte.CopyTo(WritableTypePart);
            dataByte.CopyTo(WritableDataPart);
            BinaryPrimitives.WriteUInt32BigEndian(WritableCrcPart, CalculateCrc(type, data));
        }

        public Span<byte> WritableValue => _value.AsSpan();

        private Span<byte> WritableLengthPart => _value.AsSpan().Slice(0, 4);
        private Span<byte> WritableTypePart => _value.AsSpan().Slice(4, 4);
        private Span<byte> WritableDataPart => _value.AsSpan().Slice(8, LengthPartInt);
        private Span<byte> WritableCrcPart => _value.AsSpan().Slice(8 + LengthPartInt, 4);

        public ReadOnlySpan<byte> Value => WritableValue;

        public ReadOnlySpan<byte> LengthPart => WritableLengthPart;
        public ReadOnlySpan<byte> TypePart => WritableTypePart;
        public ReadOnlySpan<byte> DataPart => WritableDataPart;
        public ReadOnlySpan<byte> CrcPart => WritableCrcPart;

        public string TypeString => Encoding.UTF8.GetString(TypePart);
        public int LengthPartInt => BinaryPrimitives.ReadInt32BigEndian(LengthPart);
        public string DataString => Encoding.UTF8.GetString(DataPart);
        public int CrcPartUInt => BinaryPrimitives.ReadInt32BigEndian(CrcPart);

        public static bool IsValid(ReadOnlySpan<byte> chunk)
        {
            if (chunk == null)
            {
                return false;
            }

            if (chunk.Length < 4)
            {
                return false;
            }

            var dataLength = BinaryPrimitives.ReadInt32BigEndian(chunk.Slice(0, 4));

            if (chunk.Length != 12 + dataLength)
            {
                return false;
            }

            var type = chunk.Slice(4, 8);
            var data = chunk.Slice(8, dataLength);

            var crc = CalculateCrc(type, data);

            // CRCをチェック
            if (crc != BinaryPrimitives.ReadInt32BigEndian(chunk.Slice(8 + dataLength, 4)))
            {
                return false;
            }

            return true;
        }

        public void SetType(ReadOnlySpan<byte> type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.Length != 4)
            {
                throw new ArgumentException(Resources.Chunk_Constructor_ChunkTypeOutOfRange);
            }

            type.CopyTo(WritableTypePart);
        }

        public void SetType(string type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            SetType(Encoding.UTF8.GetBytes(type));
        }

        public void SetData(ReadOnlySpan<byte> data)
        {
            if (data == null)
            {
                data = Array.Empty<byte>();
            }

            if (data.Length == DataPart.Length)
            {
                data.CopyTo(WritableTypePart);
                UpdateCrc();
            }
            else
            {
                ReadOnlySpan<byte> type = TypePart;

                _value = new byte[12 + data.Length];

                BinaryPrimitives.WriteInt32BigEndian(WritableLengthPart, data.Length);
                type.CopyTo(WritableTypePart);
                data.CopyTo(WritableDataPart);
                UpdateCrc();
            }
        }

        public void SetData(string type)
        {
            SetType(Encoding.UTF8.GetBytes(type));
        }

        private static uint CalculateCrc(ReadOnlySpan<byte> type, ReadOnlySpan<byte> data)
        {
            var source = new byte[type.Length + data.Length];
            type.CopyTo(source.AsSpan().Slice(0, 4));
            data.CopyTo(source.AsSpan().Slice(4, data.Length));

            return Crc32Algorithm.Compute(source);
        }

        private uint CalculateCrc(string type, string? data)
        {
            if (data == null)
            {
                data = string.Empty;
            }

            return CalculateCrc(Encoding.UTF8.GetBytes(type), Encoding.UTF8.GetBytes(data));
        }

        private void UpdateCrc() => BinaryPrimitives.WriteUInt32BigEndian(WritableCrcPart, CalculateCrc(TypePart, DataPart));

        public override string ToString()
        {
            return $"{TypeString}: {DataString}";
        }
    }
}
