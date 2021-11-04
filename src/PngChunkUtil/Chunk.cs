using System;
using System.Buffers.Binary;
using System.Text;

namespace KoyashiroKohaku.PngChunkUtil
{
    /// <summary>
    /// Chunk
    /// </summary>
    public readonly struct Chunk : IEquatable<Chunk>
    {
        private static readonly Range LENGTH_RANGE = 0..4;
        private static readonly Range CHUNK_TYPE_RANGE = 4..8;
        private static readonly Range CHUNK_DATA_RANGE = 8..^4;
        private static readonly Range CRC_RANGE = ^4..;

        private static readonly Range CRC_TARGET_RANGE = 4..^4;

        private readonly byte[]? _buffer;

        private Chunk(ReadOnlySpan<byte> buffer)
        {
            _buffer = buffer.ToArray();
        }

        public ReadOnlySpan<byte> Bytes => _buffer;
        public ReadOnlySpan<byte> LengthBytes => _buffer?[LENGTH_RANGE];
        public ReadOnlySpan<byte> ChunkTypeBytes => _buffer?[CHUNK_TYPE_RANGE];
        public ReadOnlySpan<byte> ChunkDataBytes => _buffer?[CHUNK_DATA_RANGE];
        public ReadOnlySpan<byte> CrcBytes => _buffer?[CRC_RANGE];

        public static Chunk Parse(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < 4)
            {
                throw new ArgumentException("`buffer.Length` must be grater than or equal to 4", nameof(buffer));
            }

            var chunkDataLength = BinaryPrimitives.ReadInt32BigEndian(buffer[LENGTH_RANGE]);
            if (buffer.Length != chunkDataLength + 12)
            {
                throw new ArgumentException("`Length` and `buffer.Length` do not match", nameof(buffer));
            }

            return new Chunk(buffer);
        }

        public static bool TryParse(ReadOnlySpan<byte> buffer, out Chunk chunk)
        {
            if (buffer.Length < 4)
            {
                chunk = default;
                return false;
            }

            var chunkDataLength = BinaryPrimitives.ReadInt32BigEndian(buffer[LENGTH_RANGE]);
            if (buffer.Length != chunkDataLength + 12)
            {
                chunk = default;
                return false;
            }

            chunk = new Chunk(buffer);
            return true;
        }

        public static Chunk Create(ReadOnlySpan<byte> chunkType, ReadOnlySpan<byte> chunkData)
        {
            if (chunkType.Length != 4)
            {
                throw new ArgumentException("`chunkType.Length` must be 4", nameof(chunkType));
            }

            var buffer = new byte[12 + chunkData.Length];
            var span = buffer.AsSpan();
            BinaryPrimitives.WriteInt32BigEndian(span[LENGTH_RANGE], chunkData.Length);
            chunkType.CopyTo(span[CHUNK_TYPE_RANGE]);
            chunkData.CopyTo(span[CHUNK_DATA_RANGE]);
            BinaryPrimitives.WriteUInt32BigEndian(span[CRC_RANGE], Crc32.Compute(span[CRC_TARGET_RANGE]));

            return new Chunk(buffer);
        }

        public static bool TryCreate(ReadOnlySpan<byte> chunkType, ReadOnlySpan<byte> chunkData, out Chunk chunk)
        {
            if (chunkType.Length != 4)
            {
                chunk = default;
                return false;
            }

            var buffer = new byte[12 + chunkData.Length];
            var span = buffer.AsSpan();
            BinaryPrimitives.WriteInt32BigEndian(span[LENGTH_RANGE], chunkData.Length);
            chunkType.CopyTo(span[CHUNK_TYPE_RANGE]);
            chunkData.CopyTo(span[CHUNK_DATA_RANGE]);
            BinaryPrimitives.WriteUInt32BigEndian(span[CRC_RANGE], Crc32.Compute(span[CRC_TARGET_RANGE]));

            chunk = new Chunk(buffer);
            return true;
        }

        public static Chunk Create(string chunkType, ReadOnlySpan<byte> chunkData)
        {
            var chunkTypeBytes = Encoding.UTF8.GetBytes(chunkType);

            return Create(chunkTypeBytes, chunkData);
        }

        public static bool TryCreate(string chunkType, ReadOnlySpan<byte> chunkData, out Chunk chunk)
        {
            var chunkTypeBytes = Encoding.UTF8.GetBytes(chunkType);

            return TryCreate(chunkTypeBytes, chunkData, out chunk);
        }

        public static Chunk Create(string chunkType, string chunkData)
        {
            var chunkTypeBytes = Encoding.UTF8.GetBytes(chunkType);
            var chunkDataBytes = string.IsNullOrEmpty(chunkData) ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(chunkData);

            return Create(chunkTypeBytes, chunkDataBytes);
        }

        public static bool TryCreate(string chunkType, string chunkData, out Chunk chunk)
        {
            var chunkTypeBytes = Encoding.UTF8.GetBytes(chunkType);
            var chunkDataBytes = string.IsNullOrEmpty(chunkData) ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(chunkData);

            return TryCreate(chunkTypeBytes, chunkDataBytes, out chunk);
        }

        public bool IsValid()
        {
            if (_buffer is null)
            {
                return false;
            }

            if (_buffer.Length < 4)
            {
                return false;
            }

            if (_buffer.Length != 12 + BinaryPrimitives.ReadInt32BigEndian(_buffer[LENGTH_RANGE]))
            {
                return false;
            }

            if (Crc32.Compute(_buffer[CRC_TARGET_RANGE]) != BinaryPrimitives.ReadUInt32BigEndian(_buffer[CRC_RANGE]))
            {
                return false;
            }

            return true;
        }

        public int? ChunkDataLength()
        {
            if (_buffer is null)
            {
                return default;
            }

            if (!IsValid())
            {
                return default;
            }

            return BinaryPrimitives.ReadInt32BigEndian(_buffer[LENGTH_RANGE]);
        }

        public string? ChunkType()
        {
            if (_buffer is null)
            {
                return default;
            }

            if (!IsValid())
            {
                return default;
            }

            return Encoding.UTF8.GetString(_buffer[CHUNK_TYPE_RANGE]);
        }

        public string? ChunkData()
        {
            if (_buffer is null)
            {
                return default;
            }

            if (!IsValid())
            {
                return default;
            }

            return Encoding.UTF8.GetString(_buffer[CHUNK_DATA_RANGE]);
        }

        public uint? Crc()
        {
            if (_buffer is null)
            {
                return default;
            }

            if (!IsValid())
            {
                return default;
            }

            return BinaryPrimitives.ReadUInt32BigEndian(_buffer[CRC_RANGE]);
        }

        public override string ToString()
        {
            if (!IsValid())
            {
                return "Invalid Chunk";
            }

            return $"{ChunkType()}: {ChunkData()}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is Chunk other)
            {
                return Equals(other);
            }

            return false;
        }

        public bool Equals(Chunk other)
        {
            return Bytes.SequenceEqual(other.Bytes);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_buffer);
        }

        public static bool operator ==(Chunk left, Chunk right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Chunk left, Chunk right)
        {
            return !(left == right);
        }
    }
}
