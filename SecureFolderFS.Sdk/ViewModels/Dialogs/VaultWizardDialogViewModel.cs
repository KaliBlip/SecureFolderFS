﻿using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SecureFolderFS.Sdk.ViewModels.Pages.VaultWizard;
using CommunityToolkit.Mvvm.Input;
using SecureFolderFS.Sdk.Messages.Navigation;
using SecureFolderFS.Shared.Utils;

namespace SecureFolderFS.Sdk.ViewModels.Dialogs
{
    public sealed partial class VaultWizardDialogViewModel : DialogViewModel, IRecipient<NavigationRequestedMessage>
    {
        public IMessenger Messenger { get; }

        [ObservableProperty]
        private BaseVaultWizardPageViewModel? _CurrentPageViewModel;

        public VaultWizardDialogViewModel()
        {
            Messenger = new WeakReferenceMessenger();
            Messenger.Register(this);
        }

        /// <inheritdoc/>
        public void Receive(NavigationRequestedMessage message)
        {
            CurrentPageViewModel = message.ViewModel as BaseVaultWizardPageViewModel;
        }

        [RelayCommand]
        private Task PrimaryButtonClickAsync(IEventDispatchFlag? flag, CancellationToken cancellationToken)
        {
            return CurrentPageViewModel?.PrimaryButtonClickAsync(flag, cancellationToken) ?? Task.CompletedTask;
        }

        [RelayCommand]
        private Task SecondaryButtonClickAsync(IEventDispatchFlag? flag, CancellationToken cancellationToken)
        {
            return CurrentPageViewModel?.SecondaryButtonClickAsync(flag, cancellationToken) ?? Task.CompletedTask;
        }

        [RelayCommand]
        private void GoBack()
        {
            Messenger.Send(new BackNavigationRequestedMessage());
        }
    }
}
