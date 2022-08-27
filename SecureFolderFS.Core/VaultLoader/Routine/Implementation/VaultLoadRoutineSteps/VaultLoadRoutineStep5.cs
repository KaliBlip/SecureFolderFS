﻿using SecureFolderFS.Core.DataModels;
using SecureFolderFS.Core.Discoverers;
using SecureFolderFS.Core.Helpers;
using SecureFolderFS.Core.Instance.Implementation;
using SecureFolderFS.Core.VaultLoader.Discoverers;

namespace SecureFolderFS.Core.VaultLoader.Routine.Implementation.VaultLoadRoutineSteps
{
    internal sealed class VaultLoadRoutineStep5 : BaseVaultLoadRoutineStep, IVaultLoadRoutineStep5
    {
        public VaultLoadRoutineStep5(VaultInstance vaultInstance, VaultLoadDataModel vaultLoadDataModel)
            : base(vaultInstance, vaultLoadDataModel)
        {
        }

        public IVaultLoadRoutineStep6 FindKeystoreFile(bool useExternalDiscoverer = false, IVaultKeystoreDiscoverer vaultKeystoreDiscoverer = null)
        {
            vaultLoadDataModel.VaultKeystoreStream = VaultHelpers.FindVaultFile(
               () => new FromVaultPathVaultKeystoreDiscoverer(),
               vaultFileDiscoverer => vaultFileDiscoverer.DiscoverVaultKeystore(vaultInstance.VaultPath.VaultRootPath, Constants.VAULT_KEYSTORE_FILENAME),
               useExternalDiscoverer,
               vaultKeystoreDiscoverer);

            return new VaultLoadRoutineStep6(vaultInstance, vaultLoadDataModel);
        }
    }
}
