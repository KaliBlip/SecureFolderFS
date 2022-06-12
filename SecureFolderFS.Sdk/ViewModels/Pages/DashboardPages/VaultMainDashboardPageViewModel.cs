﻿using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SecureFolderFS.Sdk.Enums;
using SecureFolderFS.Sdk.Messages;
using SecureFolderFS.Sdk.Models.Transitions;
using SecureFolderFS.Sdk.Services;
using SecureFolderFS.Sdk.ViewModels.Dashboard.Widgets;

namespace SecureFolderFS.Sdk.ViewModels.Pages.DashboardPages
{
    public sealed class VaultMainDashboardPageViewModel : BaseDashboardPageViewModel
    {
        private IFileExplorerService FileExplorerService { get; } = Ioc.Default.GetRequiredService<IFileExplorerService>();

        public VaultHealthWidgetViewModel VaultHealthWidgetViewModel { get; }

        public GraphsWidgetViewModel GraphsWidgetViewModel { get; }

        public IAsyncRelayCommand ShowInFileExplorerCommand { get; }

        public IRelayCommand LockVaultCommand { get; }

        public IRelayCommand OpenVaultPropertiesCommand { get; }

        public VaultMainDashboardPageViewModel(IMessenger messenger, VaultViewModel vaultViewModel)
            : base(messenger, vaultViewModel, VaultDashboardPageType.MainDashboardPage)
        {
            VaultHealthWidgetViewModel = new();
            GraphsWidgetViewModel = new();
            base.NavigationItemViewModel = new()
            {
                Index = 0,
                NavigationAction = first => Messenger.Send(new DashboardNavigationRequestedMessage(VaultDashboardPageType.MainDashboardPage, VaultViewModel) { Transition = new SlideTransitionModel(SlideTransitionDirection.ToRight) }),
                SectionName = vaultViewModel.VaultName
            };
            GraphsWidgetViewModel.StartReporting();

            ShowInFileExplorerCommand = new AsyncRelayCommand(ShowInFileExplorer);
            LockVaultCommand = new RelayCommand(LockVault);
            OpenVaultPropertiesCommand = new RelayCommand(OpenVaultProperties);
        }

        private async Task ShowInFileExplorer()
        {
            if (VaultViewModel.VaultInstance is not null)
            {
                await FileExplorerService.OpenPathInFileExplorerAsync(VaultViewModel.VaultInstance.SecureFolderFSInstance.MountLocation);
            }
        }

        private void LockVault()
        {
            VaultViewModel.VaultInstance?.Dispose();
            VaultViewModel.VaultInstance = null;
            Messenger.Send(new VaultLockedMessage(VaultViewModel));
            WeakReferenceMessenger.Default.Send(new VaultLockedMessage(VaultViewModel));
        }

        private void OpenVaultProperties()
        {
            Messenger.Send(new DashboardNavigationRequestedMessage(VaultDashboardPageType.DashboardPropertiesPage, VaultViewModel) { Transition = new SlideTransitionModel(SlideTransitionDirection.ToLeft)});
        }
    }
}
