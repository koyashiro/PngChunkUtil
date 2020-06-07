using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Force.Crc32;

namespace KoyashiroKohaku.PngMetaDataUtil
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
                throw new ArgumentNullException();
            }

            if (!IsValid(chunk))
            {
                throw new ArgumentException();
            }

            _value = chunk.ToArray();
        }

        public Chunk(ReadOnlySpan<byte> type, ReadOnlySpan<byte> data)
        {
            if (type == null || data == null)
            {
                throw new ArgumentNullException();
            }

            _value = new byte[12 + data.Length];

            BinaryPrimitives.WriteInt32BigEndian(WritableLengthPart, data.Length);
            type.CopyTo(WritableTypePart);
            data.CopyTo(WritableDataPart);
            UpdateCrc();
        }

        public Chunk(string type, string data)
        {
            if (type == null || data == null)
            {
                throw new ArgumentNullException();
            }

            var typeByte = Encoding.UTF8.GetBytes(type);
            var dataByte = Encoding.UTF8.GetBytes(data);

            var length = typeByte.Length;
            _value = new byte[12 + length];

            BinaryPrimitives.WriteInt32BigEndian(WritableLengthPart, length);
            typeByte.CopyTo(WritableTypePart);
            dataByte.CopyTo(WritableDataPart);
            BinaryPrimitives.WriteUInt32BigEndian(WritableCrcPart, CalculateCrc(type, data));
        }

        public Span<byte> WritableValue => _value.AsSpan();

        private Span<byte> WritableLengthPart => _value.AsSpan().Slice(0, 4);
        private Span<byte> WritableTypePart => _value.AsSpan().Slice(4, 8);
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
                throw new ArgumentNullException();
            }

            var dataLength = BinaryPrimitives.ReadInt32BigEndian(chunk.Slice(0, 4));

            // バイト長をチェック
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
                throw new ArgumentNullException();
            }

            if (type.Length != 4)
            {
                throw new ArgumentException();
            }

            type.CopyTo(WritableTypePart);
        }

        public void SetType(string type)
        {
            if (type == null)
            {
                throw new ArgumentNullException();
            }

            SetType(Encoding.UTF8.GetBytes(type));
        }

        public void SetData(ReadOnlySpan<byte> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException();
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
            if (type == null)
            {
                throw new ArgumentNullException();
            }

            SetType(Encoding.UTF8.GetBytes(type));
        }

        private static uint CalculateCrc(ReadOnlySpan<byte> type, ReadOnlySpan<byte> data)
        {
            if (type == null || data == null)
            {
                throw new ArgumentNullException();
            }

            var source = new byte[type.Length + data.Length];
            type.CopyTo(source.AsSpan().Slice(0, 4));
            data.CopyTo(source.AsSpan().Slice(4, data.Length));

            return Crc32Algorithm.Compute(source);
        }

        private uint CalculateCrc(string type, string data)
        {
            if (type == null || data == null)
            {
                throw new ArgumentNullException();
            }

            var typeByte = Encoding.UTF8.GetBytes(type).AsSpan();
            var dataByte = Encoding.UTF8.GetBytes(data).AsSpan();

            var source = new byte[type.Length + data.Length];
            typeByte.CopyTo(source.AsSpan().Slice(0, 4));
            dataByte.CopyTo(source.AsSpan().Slice(4, data.Length));

            return Crc32Algorithm.Compute(source);
        }

        private void UpdateCrc() => BinaryPrimitives.WriteUInt32BigEndian(WritableCrcPart, CalculateCrc(TypePart, DataPart));

        public override string ToString()
        {
            return $"{TypeString}: {DataString}";
        }
    }
}
