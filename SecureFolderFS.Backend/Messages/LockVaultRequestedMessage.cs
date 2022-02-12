﻿using CommunityToolkit.Mvvm.Messaging.Messages;
using SecureFolderFS.Backend.ViewModels;

namespace SecureFolderFS.Backend.Messages
{
    public sealed class LockVaultRequestedMessage : ValueChangedMessage<VaultViewModel>
    {
        public LockVaultRequestedMessage(VaultViewModel value)
            : base(value)
        {
        }
    }
}
