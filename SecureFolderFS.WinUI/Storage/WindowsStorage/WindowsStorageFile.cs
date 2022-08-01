﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using SecureFolderFS.Sdk.Storage;
using SecureFolderFS.Sdk.Storage.LocatableStorage;
using SecureFolderFS.Sdk.Storage.ModifiableStorage;

namespace SecureFolderFS.WinUI.Storage.WindowsStorage
{
    /// <inheritdoc cref="IFile"/>
    internal sealed class WindowsStorageFile : WindowsStorable<StorageFile>, ILocatableFile, IModifiableFile
    {
        public WindowsStorageFile(StorageFile storage)
            : base(storage)
        {
        }

        /// <inheritdoc/>
        public async Task<Stream> OpenStreamAsync(FileAccess access, CancellationToken cancellationToken = default)
        {
            var fileAccessMode = GetFileAccessMode(access);
            var storageOpenOptions = GetStorageOpenOptions(FileShare.None);

            var winrtStreamTask = storage.OpenAsync(fileAccessMode, storageOpenOptions).AsTask(cancellationToken);
            var winrtStream = await winrtStreamTask;

            return winrtStream.AsStream();
        }

        /// <inheritdoc/>
        public override async Task<ILocatableFolder?> GetParentAsync(CancellationToken cancellationToken = default)
        {
            var parentFolderTask = storage.GetParentAsync().AsTask(cancellationToken);
            var parentFolder = await parentFolderTask;

            return new WindowsStorageFolder(parentFolder);
        }

        private static FileAccessMode GetFileAccessMode(FileAccess access)
        {
            return access switch
            {
                FileAccess.Read => FileAccessMode.Read,
                FileAccess.Write => FileAccessMode.ReadWrite,
                FileAccess.ReadWrite => FileAccessMode.ReadWrite,
                _ => throw new ArgumentOutOfRangeException(nameof(access))
            };
        }

        private static StorageOpenOptions GetStorageOpenOptions(FileShare share)
        {
            return share switch
            {
                FileShare.Read => StorageOpenOptions.AllowOnlyReaders,
                FileShare.Write => StorageOpenOptions.AllowReadersAndWriters,
                FileShare.ReadWrite => StorageOpenOptions.AllowReadersAndWriters,
                FileShare.Inheritable => StorageOpenOptions.None,
                FileShare.Delete => StorageOpenOptions.None,
                FileShare.None => StorageOpenOptions.None,
                _ => throw new ArgumentOutOfRangeException(nameof(share))
            };
        }
    }
}
