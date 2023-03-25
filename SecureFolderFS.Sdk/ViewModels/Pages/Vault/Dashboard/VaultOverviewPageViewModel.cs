﻿using CommunityToolkit.Mvvm.DependencyInjection;
using SecureFolderFS.Sdk.Enums;
using SecureFolderFS.Sdk.Models;
using SecureFolderFS.Sdk.Services;
using SecureFolderFS.Sdk.Storage.LocatableStorage;
using SecureFolderFS.Sdk.ViewModels.Controls;
using SecureFolderFS.Sdk.ViewModels.Controls.Widgets;
using SecureFolderFS.Sdk.ViewModels.Vault;
using System.Threading;
using System.Threading.Tasks;

namespace SecureFolderFS.Sdk.ViewModels.Pages.Vault.Dashboard
{
    public sealed class VaultOverviewPageViewModel : BaseDashboardPageViewModel
    {
        private ISettingsService SettingsService { get; } = Ioc.Default.GetRequiredService<ISettingsService>();

        private IFileExplorerService FileExplorerService { get; } = Ioc.Default.GetRequiredService<IFileExplorerService>();

        public WidgetsListViewModel WidgetsViewModel { get; }

        public VaultControlsViewModel VaultControlsViewModel { get; }

        public VaultOverviewPageViewModel(UnlockedVaultViewModel unlockedVaultViewModel, IStateNavigationModel navigationModel)
            : base(unlockedVaultViewModel, navigationModel)
        {
            WidgetsViewModel = new(unlockedVaultViewModel.UnlockedVaultModel, unlockedVaultViewModel.VaultViewModel.WidgetsContextModel);
            VaultControlsViewModel = new(unlockedVaultViewModel, navigationModel);
        }

        /// <inheritdoc/>
        public override async Task InitAsync(CancellationToken cancellationToken = default)
        {
            if (SettingsService.UserSettings.OpenFolderOnUnlock && UnlockedVaultViewModel.UnlockedVaultModel.RootFolder is ILocatableFolder rootFolder)
                _ = FileExplorerService.OpenInFileExplorerAsync(rootFolder, cancellationToken);

            await WidgetsViewModel.InitAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            WidgetsViewModel.Dispose();
        }
    }
}
