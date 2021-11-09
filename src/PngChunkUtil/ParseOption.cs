namespace Koyashiro.PngChunkUtil
{
    public enum ChunkTypeFilter
    {
        All,
        CriticalChunkOnly,
        AncillaryChunkOnly,
        AdditionalChunkOnly,
        CriticalChunkAndAncillaryChunkOnly,
        WithoutCriticalChunk,
        WithoutAncillaryChunk,
        WithoutAdditionalChunk,
    }
}
