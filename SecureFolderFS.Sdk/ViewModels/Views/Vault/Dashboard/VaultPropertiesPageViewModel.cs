﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using SecureFolderFS.Sdk.Extensions;
using SecureFolderFS.Sdk.Services;
using SecureFolderFS.Sdk.ViewModels.Dialogs;
using SecureFolderFS.Sdk.ViewModels.Vault;
using System.Threading;
using System.Threading.Tasks;

namespace SecureFolderFS.Sdk.ViewModels.Views.Vault.Dashboard
{
    public sealed partial class VaultPropertiesPageViewModel : BaseDashboardPageViewModel
    {
        private IDialogService DialogService { get; } = Ioc.Default.GetRequiredService<IDialogService>();

        [ObservableProperty] private string? _ContentCipherName;
        [ObservableProperty] private string? _FileNameCipherName;

        public VaultPropertiesPageViewModel(UnlockedVaultViewModel unlockedVaultViewModel, INavigationService dashboardNavigationService)
            : base(unlockedVaultViewModel, dashboardNavigationService)
        {
            var contentCipherId = unlockedVaultViewModel.UnlockedVaultModel.VaultInfoModel.ContentCipherId;
            var fileNameCipherId = unlockedVaultViewModel.UnlockedVaultModel.VaultInfoModel.FileNameCipherId;

            ContentCipherName = contentCipherId == string.Empty ? "None" : (contentCipherId ?? "Unknown");
            FileNameCipherName = fileNameCipherId == string.Empty ? "None" : (fileNameCipherId ?? "Unknown");
        }

        /// <inheritdoc/>
        public override Task InitAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        [RelayCommand]
        private async Task ChangePasswordAsync()
        {
            using var viewModel = new PasswordChangeDialogViewModel(UnlockedVaultViewModel.VaultViewModel.VaultModel);
            await DialogService.ShowDialogAsync(viewModel);
        }
    }
}
