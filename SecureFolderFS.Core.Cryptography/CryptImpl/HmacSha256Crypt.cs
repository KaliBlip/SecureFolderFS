﻿using SecureFolderFS.Core.Cryptography.Cipher;
using System;
using System.Security.Cryptography;

namespace SecureFolderFS.Core.Cryptography.CryptImpl
{
    /// <inheritdoc cref="IHmacSha256Crypt"/>
    public sealed class HmacSha256Crypt : IHmacSha256Crypt
    {
        /// <inheritdoc/>
        public IHmacSha256Instance GetInstance()
        {
            return new HmacSha256Instance();
        }

        private sealed class HmacSha256Instance : IHmacSha256Instance
        {
            private IncrementalHash? _incrementalHash;

            /// <inheritdoc/>
            public void InitializeHmac(ReadOnlySpan<byte> key)
            {
                _incrementalHash = IncrementalHash.CreateHMAC(HashAlgorithmName.SHA256, key);
            }

            /// <inheritdoc/>
            public void Update(ReadOnlySpan<byte> bytes)
            {
                ArgumentNullException.ThrowIfNull(_incrementalHash);
                _incrementalHash.AppendData(bytes);
            }

            /// <inheritdoc/>
            public void DoFinal(ReadOnlySpan<byte> bytes)
            {
                ArgumentNullException.ThrowIfNull(_incrementalHash);
                _incrementalHash.AppendData(bytes);
            }

            /// <inheritdoc/>
            public void GetHash(Span<byte> destination)
            {
                ArgumentNullException.ThrowIfNull(_incrementalHash);
                _incrementalHash.GetCurrentHash(destination);
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                _incrementalHash?.Dispose();
                _incrementalHash = null;
            }
        }
    }
}
