﻿using System.Threading.Tasks;

namespace SecureFolderFS.Sdk.Storage
{
    public interface IFileSystemService
    {
        /// <summary>
        /// Checks and requests permission to access file system.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. If access is granted returns true, otherwise false.</returns>
        Task<bool> IsFileSystemAccessible();

        /// <summary>
        /// Check if file exists at specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. Result is true if the file exists, otherwise false.</returns>
        Task<bool> FileExistsAsync(string path);

        /// <summary>
        /// Check if directory exists at specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. Result is true if the directory exists, otherwise false.</returns>
        Task<bool> DirectoryExistsAsync(string path);

        /// <summary>
        /// Creates all directories at specified <paramref name="folderName"/> path.
        /// </summary>
        /// <param name="folderName">The directory to create.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. Returns <see cref="IFolder"/> for the created directory.</returns>
        Task<IFolder> CreateDirectoryAsync(string folderName);

        /// <summary>
        /// Gets the folder at the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to the folder.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. If folder is found and access is granted, returns <see cref="IFolder"/>, otherwise null.</returns>
        Task<IFolder?> GetFolderFromPathAsync(string path);

        /// <summary>
        /// Gets the file at the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. If file is found and access is granted, returns <see cref="IFile"/>, otherwise null.</returns>
        Task<IFile?> GetFileFromPathAsync(string path);
    }
}