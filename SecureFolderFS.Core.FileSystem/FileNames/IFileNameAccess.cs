﻿using System;
using SecureFolderFS.Core.FileSystem.Directories;

namespace SecureFolderFS.Core.FileSystem.FileNames
{
    /// <summary>
    /// Accesses cleartext and ciphertext names of files and folders found on the encrypting file system.
    /// </summary>
    public interface IFileNameAccess
    {
        /// <summary>
        /// Gets cleartext name from associated <paramref name="ciphertextName"/>.
        /// </summary>
        /// <param name="ciphertextName">The associated ciphertext name.</param>
        /// <param name="directoryId">The ID of directory where the file/folder is stored.</param>
        /// <returns>If successful, returns a cleartext representation of the name, otherwise empty.</returns>
        ReadOnlySpan<char> GetCleartextName(ReadOnlySpan<char> ciphertextName, DirectoryId directoryId);

        /// <summary>
        /// Gets ciphertext name from associated <paramref name="cleartextName"/>.
        /// </summary>
        /// <param name="cleartextName">The associated cleartext name.</param>
        /// <param name="directoryId">The ID of directory where the file/folder is stored.</param>
        /// <returns>If successful, returns a ciphertext representation of the name, otherwise empty.</returns>
        ReadOnlySpan<char> GetCiphertextName(ReadOnlySpan<char> cleartextName, DirectoryId directoryId);
    }
}