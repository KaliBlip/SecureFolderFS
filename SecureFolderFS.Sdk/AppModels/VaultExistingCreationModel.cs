﻿using CommunityToolkit.Mvvm.DependencyInjection;
using SecureFolderFS.Sdk.Models;
using SecureFolderFS.Sdk.Services;
using SecureFolderFS.Sdk.Storage;
using SecureFolderFS.Shared.Helpers;
using SecureFolderFS.Shared.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SecureFolderFS.Sdk.AppModels
{
    /// <inheritdoc cref="IVaultExistingCreationModel"/>
    public sealed class VaultExistingCreationModel : IVaultExistingCreationModel
    {
        private readonly IAsyncValidator<IFolder> _vaultValidator;
        private IFolder? _vaultFolder;

        private IVaultService VaultService { get; } = Ioc.Default.GetRequiredService<IVaultService>();

        public VaultExistingCreationModel()
        {
            _vaultValidator = VaultService.GetVaultValidator();
        }

        /// <inheritdoc/>
        public async Task<IResult> SetFolderAsync(IFolder folder, CancellationToken cancellationToken = default)
        {
            _vaultFolder = folder;
            return await _vaultValidator.ValidateAsync(folder, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IResult<IVaultModel?>> DeployAsync(CancellationToken cancellationToken = default)
        {
            if (_vaultFolder is null)
                return Task.FromResult<IResult<IVaultModel?>>(new CommonResult<IVaultModel?>(null));

            // Create vault model
            IVaultModel vaultModel = new LocalVaultModel(_vaultFolder);

            return Task.FromResult<IResult<IVaultModel?>>(new CommonResult<IVaultModel?>(vaultModel));
        }
    }
}
