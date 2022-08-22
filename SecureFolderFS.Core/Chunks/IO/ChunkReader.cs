﻿using System;
using SecureFolderFS.Core.Security;
using SecureFolderFS.Core.FileHeaders;
using SecureFolderFS.Core.Sdk.Tracking;
using SecureFolderFS.Core.Streams.Management;

namespace SecureFolderFS.Core.Chunks.IO
{
    internal sealed class ChunkReader : IChunkReaderDeprecated
    {
        private readonly ISecurity _security;

        private readonly IFileHeader _fileHeader;

        private readonly IChunkFactory _chunkFactory;

        private readonly CiphertextStreamsManager _ciphertextStreamsManager;

        private readonly IFileSystemStatsTracker _fileSystemStatsTracker;

        private bool _disposed;

        public ChunkReader(ISecurity security, IFileHeader fileHeader, IChunkFactory chunkFactory, CiphertextStreamsManager ciphertextStreamsManager, IFileSystemStatsTracker fileSystemStatsTracker)
        {
            _security = security;
            _fileHeader = fileHeader;
            _chunkFactory = chunkFactory;
            _ciphertextStreamsManager = ciphertextStreamsManager;
            _fileSystemStatsTracker = fileSystemStatsTracker;
        }

        public ICleartextChunk ReadChunk(long chunkNumber)
        {
            AssertNotDisposed();

            // Calculate
            var payloadSize = _security.ContentCryptor.FileContentCryptor.ChunkCleartextSize;
            var chunkSize = _security.ContentCryptor.FileContentCryptor.ChunkFullCiphertextSize;
            var ciphertextPosition = _security.ContentCryptor.FileHeaderCryptor.HeaderSize + (chunkNumber * chunkSize);

            // Initialize buffer
            var ciphertextBuffer = new byte[chunkSize];
            var ciphertextBufferMemory = ciphertextBuffer.AsMemory();

            // Read from stream
            var ciphertextFileStream = _ciphertextStreamsManager.GetReadOnlyStreamInstance();
            ciphertextFileStream.Position = ciphertextPosition;
            var read = ciphertextFileStream.Read(ciphertextBufferMemory.Span);

            // Check for end-of-file
            if (read == Constants.IO.FILE_EOF)
            {
                return _chunkFactory.FromCleartextChunkBuffer(new byte[payloadSize], 0);
            }

            _fileSystemStatsTracker?.AddBytesRead(read);

            // Decrypt
            var cleartextChunk = _security.ContentCryptor.FileContentCryptor.DecryptChunk(
                _chunkFactory.FromCiphertextChunkBuffer(ciphertextBufferMemory.Slice(0, read)),
                chunkNumber,
                _fileHeader,
                true);

            _fileSystemStatsTracker?.AddBytesDecrypted(cleartextChunk.ActualLength);

            return cleartextChunk;
        }

        private void AssertNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        public void Dispose()
        {
            _disposed = true;
            _ciphertextStreamsManager?.Dispose();
        }
    }
}
