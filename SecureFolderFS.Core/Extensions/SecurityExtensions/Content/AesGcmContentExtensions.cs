﻿using System;
using static SecureFolderFS.Core.Constants.Security.Chunks.AesGcm;

namespace SecureFolderFS.Core.Extensions.SecurityExtensions.Content
{
    internal static class AesGcmContentExtensions
    {
        public static ReadOnlySpan<byte> GetChunkNonce(this ReadOnlySpan<byte> ciphertextChunk)
        {
            return ciphertextChunk.Slice(0, CHUNK_NONCE_SIZE);
        }

        public static ReadOnlySpan<byte> GetChunkPayload(this ReadOnlySpan<byte> ciphertextChunk)
        {
            return ciphertextChunk.Slice(CHUNK_NONCE_SIZE, ciphertextChunk.Length - (CHUNK_NONCE_SIZE + CHUNK_TAG_SIZE));
        }

        public static ReadOnlySpan<byte> GetChunkTag(this ReadOnlySpan<byte> ciphertextChunk)
        {
            return ciphertextChunk.Slice(ciphertextChunk.Length - CHUNK_TAG_SIZE);
        }
    }
}
