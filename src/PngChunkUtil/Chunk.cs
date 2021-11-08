using System;
using System.Buffers.Binary;
using System.IO.Hashing;
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

        private readonly ReadOnlyMemory<byte> _buffer;

        private Chunk(ReadOnlyMemory<byte> buffer)
        {
            _buffer = buffer;
        }

        public ReadOnlySpan<byte> Bytes => _buffer.Span;
        public ReadOnlySpan<byte> LengthBytes => _buffer.IsEmpty ? Span<byte>.Empty : _buffer.Span[LENGTH_RANGE];
        public ReadOnlySpan<byte> ChunkTypeBytes => _buffer.IsEmpty ? Span<byte>.Empty : _buffer.Span[CHUNK_TYPE_RANGE];
        public ReadOnlySpan<byte> ChunkDataBytes => _buffer.IsEmpty ? Span<byte>.Empty : _buffer.Span[CHUNK_DATA_RANGE];
        public ReadOnlySpan<byte> CrcBytes => _buffer.IsEmpty ? Span<byte>.Empty : _buffer.Span[CRC_RANGE];

        public int? ChunkDataLength => IsValid() ? BinaryPrimitives.ReadInt32BigEndian(_buffer.Span[LENGTH_RANGE]) : default(int?);
        public string? ChunkType() => IsValid() ? Encoding.UTF8.GetString(_buffer.Span[CHUNK_TYPE_RANGE]) : default(string?);
        public string? ChunkData() => IsValid() ? Encoding.UTF8.GetString(_buffer.Span[CHUNK_DATA_RANGE]) : default(string?);
        public uint? Crc() => IsValid() ? BinaryPrimitives.ReadUInt32BigEndian(_buffer.Span[CRC_RANGE]) : default(uint?);

        public static Chunk Parse(ReadOnlyMemory<byte> input)
        {
            if (input.Length < 4)
            {
                throw new ArgumentException("`input.Length` must be grater than or equal to 4", nameof(input));
            }

            var chunkDataLength = BinaryPrimitives.ReadInt32BigEndian(input.Span[LENGTH_RANGE]);
            if (input.Length != chunkDataLength + 12)
            {
                throw new ArgumentException("`Length` and `input.Length` do not match", nameof(input));
            }

            return new Chunk(input);
        }

        public static bool TryParse(ReadOnlyMemory<byte> input, out Chunk chunk)
        {
            if (input.Length < 4)
            {
                chunk = default;
                return false;
            }

            var chunkDataLength = BinaryPrimitives.ReadInt32BigEndian(input.Span[LENGTH_RANGE]);
            if (input.Length != chunkDataLength + 12)
            {
                chunk = default;
                return false;
            }

            chunk = new Chunk(input);
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

            Span<byte> hash = Crc32.Hash(span[CRC_TARGET_RANGE]);
            hash.Reverse();
            hash.CopyTo(span[CRC_RANGE]);

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

            Span<byte> hash = Crc32.Hash(span[CRC_TARGET_RANGE]);
            hash.Reverse();
            hash.CopyTo(span[CRC_RANGE]);

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
            if (_buffer.Length < 4)
            {
                return false;
            }

            if (_buffer.Length != 12 + BinaryPrimitives.ReadInt32BigEndian(_buffer.Span[LENGTH_RANGE]))
            {
                return false;
            }

            Span<byte> hash = Crc32.Hash(_buffer.Span[CRC_TARGET_RANGE]);
            hash.Reverse();
            if (!hash.SequenceEqual(_buffer.Span[CRC_RANGE]))
            {
                return false;
            }

            return true;
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
