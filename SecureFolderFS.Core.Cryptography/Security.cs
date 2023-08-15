﻿using SecureFolderFS.Core.Cryptography.ContentCrypt;
using SecureFolderFS.Core.Cryptography.Enums;
using SecureFolderFS.Core.Cryptography.HeaderCrypt;
using SecureFolderFS.Core.Cryptography.NameCrypt;
using SecureFolderFS.Core.Cryptography.SecureStore;
using System;

namespace SecureFolderFS.Core.Cryptography
{
    /// <summary>
    /// Represents a security object used for encrypting and decrypting data in SecureFolderFS.
    /// </summary>
    public sealed class Security : IDisposable
    {
        private readonly SecretKey _encKey;
        private readonly SecretKey _macKey;

        // TODO: Needs docs
        public required IHeaderCrypt HeaderCrypt { get; init; }

        public required IContentCrypt ContentCrypt { get; init; }

        public required INameCrypt? NameCrypt { get; init; }

        private Security(SecretKey encKey, SecretKey macKey)
        {
            _encKey = encKey;
            _macKey = macKey;
        }

        public SecretKey CopyEncryptionKey()
        {
            return _encKey.CreateCopy();
        }

        public SecretKey CopyMacKey()
        {
            return _macKey.CreateCopy();
        }

        /// <summary>
        /// Creates a new instance of <see cref="Security"/> object that provides content encryption and cipher access.
        /// </summary>
        /// <param name="encKey">The DEK key that this class takes ownership of.</param>
        /// <param name="macKey">The MAC key that this class takes ownership of.</param>
        /// <param name="contentCipher">The content cipher type.</param>
        /// <param name="fileNameCipher">The file name cipher type.</param>
        /// <returns>A new <see cref="Security"/> object.</returns>
        public static Security CreateNew(SecretKey encKey, SecretKey macKey, ContentCipherScheme contentCipher, FileNameCipherScheme fileNameCipher)
        {
            // Initialize crypt implementation
            IHeaderCrypt headerCrypt = contentCipher switch
            {
                ContentCipherScheme.AES_CTR_HMAC => new AesCtrHmacHeaderCrypt(encKey, macKey),
                ContentCipherScheme.AES_GCM => new AesGcmHeaderCrypt(encKey, macKey),
                ContentCipherScheme.XChaCha20_Poly1305 => new XChaChaHeaderCrypt(encKey, macKey),
                _ => throw new ArgumentOutOfRangeException(nameof(contentCipher))
            };
            IContentCrypt contentCrypt = contentCipher switch
            {
                ContentCipherScheme.AES_CTR_HMAC => new AesCtrHmacContentCrypt(macKey),
                ContentCipherScheme.AES_GCM => new AesGcmContentCrypt(),
                ContentCipherScheme.XChaCha20_Poly1305 => new XChaChaContentCrypt(),
                _ => throw new ArgumentOutOfRangeException(nameof(contentCipher))
            };
            INameCrypt? nameCrypt = fileNameCipher switch
            {
                FileNameCipherScheme.AES_SIV => new AesSivNameCrypt(encKey, macKey),
                FileNameCipherScheme.None => null,
                _ => throw new ArgumentOutOfRangeException(nameof(fileNameCipher))
            };

            return new(encKey, macKey)
            {
                ContentCrypt = contentCrypt,
                HeaderCrypt = headerCrypt,
                NameCrypt = nameCrypt
            };
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _encKey.Dispose();
            _macKey.Dispose();

            ContentCrypt.Dispose();
            HeaderCrypt.Dispose();
            NameCrypt?.Dispose();
        }
    }
}
