using Force.Crc32;
using System;
using System.Buffers.Binary;
using System.Text;

namespace KoyashiroKohaku.PngChunkUtil
{
    /// <summary>
    /// Chunk
    /// </summary>
    public struct Chunk : IEquatable<Chunk>
    {
        private readonly byte[] _buffer;
        private readonly Range _range;

        public Chunk(byte[] chunk)
        {
            if (chunk is null)
            {
                throw new ArgumentNullException(nameof(chunk));
            }

            if (!InternalIsValid(chunk))
            {
                throw new ArgumentException();
            }

            _buffer = chunk;
            _range = ..;
        }

        internal Chunk(byte[] source, Range range)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (range.End.Value - range.Start.Value < 12)
            {
                throw new ArgumentException();
            }

            if (!IsValid(source[range]))
            {
                throw new ArgumentException();
            }

            _buffer = source;
            _range = range;
        }

        public ReadOnlySpan<byte> Value => _buffer.AsSpan(_range);
        public ReadOnlySpan<byte> LengthPart => Value[..4];
        public ReadOnlySpan<byte> TypePart => Value[4..8];
        public ReadOnlySpan<byte> DataPart => Value[8..^4];
        public ReadOnlySpan<byte> CrcPart => Value[^4..];

        public string TypeString => Encoding.UTF8.GetString(TypePart);
        public int DataLength => BinaryPrimitives.ReadInt32BigEndian(LengthPart);
        public string DataString => Encoding.UTF8.GetString(DataPart);
        public uint Crc => BinaryPrimitives.ReadUInt32BigEndian(CrcPart);

        public static bool TryCreate(byte[] source, out Chunk chunk)
        {
            if (source is null)
            {
                chunk = default;
                return false;
            }

            if (!InternalIsValid(source))
            {
                chunk = default;
                return false;
            }

            chunk = new Chunk(source);
            return true;
        }

        public static bool TryCreate(byte[] source, Range range, out Chunk chunk)
        {
            if (source is null)
            {
                chunk = default;
                return false;
            }

            if (range.End.Value - range.Start.Value < 12)
            {
                chunk = default;
                return false;
            }

            if (!InternalIsValid(source.AsSpan(range)))
            {
                chunk = default;
                return false;
            }

            chunk = new Chunk(source, range);
            return true;
        }

        private static bool IsValid(byte[] chunk)
        {
            if (chunk is null)
            {
                return false;
            }

            return InternalIsValid(chunk);
        }

        private static bool IsValid(ReadOnlySpan<byte> chunk)
        {
            return InternalIsValid(chunk);
        }

        public bool IsValid()
        {
            return this != default;
        }

        internal static bool InternalIsValid(ReadOnlySpan<byte> chunk)
        {
            if (chunk.Length < 4)
            {
                return false;
            }

            if (chunk.Length != 12 + BinaryPrimitives.ReadInt32BigEndian(chunk[0..4]))
            {
                return false;
            }

            if (CalculateCrc(chunk[4..^4]) != BinaryPrimitives.ReadUInt32BigEndian(chunk[^4..]))
            {
                return false;
            }

            return true;
        }

        private static uint CalculateCrc(ReadOnlySpan<byte> typeDataChunk)
        {
            Span<byte> source = typeDataChunk.Length <= 128 ? stackalloc byte[typeDataChunk.Length] : new byte[typeDataChunk.Length];

            typeDataChunk.CopyTo(source);

            return Crc32Algorithm.Compute(source.ToArray());
        }

        public static bool IsCriticalChunk(ReadOnlySpan<byte> chunkType)
        {
            return chunkType.SequenceEqual(CriticalChunk.IHDR)
                || chunkType.SequenceEqual(CriticalChunk.PLTE)
                || chunkType.SequenceEqual(CriticalChunk.IDAT)
                || chunkType.SequenceEqual(CriticalChunk.IEND);
        }

        public static bool IsAncillaryChunk(ReadOnlySpan<byte> chunkType)
        {
            return chunkType.SequenceEqual(AncillaryChunk.CHRM)
                || chunkType.SequenceEqual(AncillaryChunk.GAMA)
                || chunkType.SequenceEqual(AncillaryChunk.ICCP)
                || chunkType.SequenceEqual(AncillaryChunk.SBIT)
                || chunkType.SequenceEqual(AncillaryChunk.SRGB)
                || chunkType.SequenceEqual(AncillaryChunk.BKGD)
                || chunkType.SequenceEqual(AncillaryChunk.HIST)
                || chunkType.SequenceEqual(AncillaryChunk.TRNS)
                || chunkType.SequenceEqual(AncillaryChunk.PHYS)
                || chunkType.SequenceEqual(AncillaryChunk.TIME)
                || chunkType.SequenceEqual(AncillaryChunk.ITXT)
                || chunkType.SequenceEqual(AncillaryChunk.TEXT)
                || chunkType.SequenceEqual(AncillaryChunk.ZTXT);
        }

        public static bool IsAdditionalChunk(ReadOnlySpan<byte> chunkType)
        {
            return !IsCriticalChunk(chunkType) && !IsAncillaryChunk(chunkType);
        }

        public override string ToString()
        {
            return $"{TypeString}: {DataString}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Chunk other)
            {
                return Equals(other);
            }

            return false;
        }

        public bool Equals(Chunk other)
        {
            return Value.SequenceEqual(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_buffer, _range);
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
