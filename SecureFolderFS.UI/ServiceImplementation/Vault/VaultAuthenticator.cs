﻿using SecureFolderFS.Sdk.AppModels;
using SecureFolderFS.Sdk.Enums;
using SecureFolderFS.Sdk.Services.Vault;
using SecureFolderFS.Sdk.Storage;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SecureFolderFS.UI.ServiceImplementation.Vault
{
    public sealed class VaultAuthenticator : IVaultAuthenticator
    {
        /// <inheritdoc/>
        public async IAsyncEnumerable<AuthenticationModel> GetAuthenticationAsync(IFolder vaultFolder, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;

            yield return new("NAME_PASSWORD", AuthenticationType.Password, null);
            //yield return new("key file", AuthenticationType.Other, new KeyFileAuthenticator());
        }
    }
}
