﻿using System;

namespace SecureFolderFS.Core.Chunks
{
    /// <summary>
    /// Provides access to cleartext chunks data in individual files.
    /// </summary>
    internal interface IChunkAccess : IDisposable
    {
        /// <summary>
        /// Copies bytes from chunk into <paramref name="destination"/>.
        /// </summary>
        /// <param name="chunkNumber">The number of chunk to copy from.</param>
        /// <param name="destination">The destination buffer to copy to.</param>
        /// <param name="offsetInChunk">The offset in chunk to start copying from.</param>
        /// <returns>The amount of bytes copied.</returns>
        int CopyFromChunk(long chunkNumber, Span<byte> destination, int offsetInChunk);

        /// <summary>
        /// Copies bytes from <paramref name="source"/> into chunk.
        /// </summary>
        /// <param name="chunkNumber">The number of chunk to copy to.</param>
        /// <param name="source">The source buffer to copy from.</param>
        /// <param name="offsetInChunk">The offset in chunk to start copying to.</param>
        /// <returns>The amount of bytes copied.</returns>
        int CopyToChunk(long chunkNumber, ReadOnlySpan<byte> source, int offsetInChunk);

        /// <summary>
        /// Flushes outstanding chunks to disk.
        /// </summary>
        void Flush();
    }
}