﻿using System;
using SecureFolderFS.Core.Chunks.IO;
using SecureFolderFS.Core.Sdk.Tracking;

namespace SecureFolderFS.Core.Chunks.Receivers
{
    internal abstract class BaseChunkReceiver : IChunkReceiver
    {
        protected readonly IChunkReaderDeprecated chunkReader;

        protected readonly IChunkWriterDeprecated chunkWriter;

        protected readonly IFileSystemStatsTracker fileSystemStatsTracker;

        private bool _disposed;

        protected BaseChunkReceiver(IChunkReaderDeprecated chunkReader, IChunkWriterDeprecated chunkWriter, IFileSystemStatsTracker fileSystemStatsTracker)
        {
            this.chunkReader = chunkReader;
            this.chunkWriter = chunkWriter;
            this.fileSystemStatsTracker = fileSystemStatsTracker;
        }

        public virtual ICleartextChunk GetChunk(long chunkNumber)
        {
            AssertNotDisposed();

            fileSystemStatsTracker?.AddChunkAccess();

            return chunkReader.ReadChunk(chunkNumber);
        }

        public virtual void SetChunk(long chunkNumber, ICleartextChunk cleartextChunk)
        {
            AssertNotDisposed();

            chunkWriter.WriteChunk(chunkNumber, cleartextChunk);
        }

        public abstract void Flush();

        protected void AssertNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        public virtual void Dispose()
        {
            _disposed = true;
            chunkReader?.Dispose();
            chunkWriter?.Dispose();
        }
    }
}
