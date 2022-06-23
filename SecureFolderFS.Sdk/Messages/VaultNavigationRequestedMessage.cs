﻿using SecureFolderFS.Sdk.Models.Transitions;
using SecureFolderFS.Sdk.ViewModels;
using SecureFolderFS.Sdk.ViewModels.Pages;

namespace SecureFolderFS.Sdk.Messages
{
    public sealed class VaultNavigationRequestedMessage : ValueMessage<BasePageViewModel?>
    {
        public TransitionModel? Transition { get; init; }

        public VaultViewModel VaultViewModel { get; }

        public VaultNavigationRequestedMessage(VaultViewModel vaultModel, BasePageViewModel? value = null)
            : base(value)
        {
            VaultViewModel = vaultModel;
        }
    }
}