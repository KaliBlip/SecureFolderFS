﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Media.Animation;
using SecureFolderFS.Shared.Extensions;
using SecureFolderFS.Sdk.Messages;
using SecureFolderFS.Sdk.Models;
using SecureFolderFS.Sdk.Models.Transitions;
using SecureFolderFS.Sdk.ViewModels;
using SecureFolderFS.Sdk.ViewModels.Pages;
using SecureFolderFS.WinUI.Helpers;
using SecureFolderFS.WinUI.Views;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SecureFolderFS.WinUI.UserControls
{
    public sealed partial class NavigationControl : UserControl,
        IRecipient<VaultLockedMessage>,
        IRecipient<NavigationRequestedMessage>,
        IRecipient<RemoveVaultRequestedMessage>,
        IRecipient<AddVaultRequestedMessage>
    {
        private Dictionary<VaultIdModel, BasePageViewModel?> NavigationDestinations { get; }

        public NavigationControl()
        {
            InitializeComponent();

            NavigationDestinations = new();

            WeakReferenceMessenger.Default.Register<NavigationRequestedMessage>(this);
            WeakReferenceMessenger.Default.Register<RemoveVaultRequestedMessage>(this);
            WeakReferenceMessenger.Default.Register<AddVaultRequestedMessage>(this);
            WeakReferenceMessenger.Default.Register<VaultLockedMessage>(this);
        }

        public async void Receive(NavigationRequestedMessage message)
        {
            await Navigate(message.VaultViewModel, message.Value, message.Transition);
        }

        public void Receive(RemoveVaultRequestedMessage message)
        {
            NavigationDestinations.Remove(message.Value, out var viewModel);
            viewModel?.Dispose();

            if (NavigationDestinations.IsEmpty())
            {
                // TODO: Navigate to the start page
            }
        }

        public void Receive(AddVaultRequestedMessage message)
        {
            NavigationDestinations.AddOrSet(message.Value.VaultIdModel);
        }

        public async void Receive(VaultLockedMessage message)
        {
            NavigationDestinations[message.Value.VaultIdModel]?.Dispose();
            NavigationDestinations[message.Value.VaultIdModel] = null;
            await Navigate(message.Value, null, new DrillOutTransitionModel());
        }

        private async Task Navigate(VaultIdModel vaultIdModel, TransitionModel? transition = null)
        {
            NavigationDestinations.SetAndGet(vaultIdModel, out var basePageViewModel, () => throw new InvalidOperationException("Could not navigate - insufficient parameters."));
            PageViewModel = basePageViewModel!;
            if (!PageViewModel.Messenger.IsRegistered<VaultLockedMessage>(this))
            {
                PageViewModel.Messenger.Register<VaultLockedMessage>(this);
            }

            await Navigate(PageViewModel, transition);

            WeakReferenceMessenger.Default.Send(new NavigationFinishedMessage(PageViewModel));
        }

        private async Task Navigate(VaultViewModel vaultViewModel, BasePageViewModel? basePageViewModel, TransitionModel? transition = null)
        {
            if (basePageViewModel is null)
            {
                NavigationDestinations.SetAndGet(vaultViewModel.VaultIdModel, out basePageViewModel, () => new VaultLoginPageViewModel(vaultViewModel));
                PageViewModel = basePageViewModel!;
                if (!PageViewModel.Messenger.IsRegistered<VaultLockedMessage>(this))
                {
                    PageViewModel.Messenger.Register<VaultLockedMessage>(this);
                }
            }
            else
            {
                if (!NavigationDestinations.SetAndGet(vaultViewModel.VaultIdModel, out _, () => basePageViewModel))
                {
                    // Wasn't updated, do it manually..
                    NavigationDestinations[vaultViewModel.VaultIdModel] = basePageViewModel;
                    PageViewModel = basePageViewModel;
                }
            }

            await Navigate(PageViewModel, transition);

            WeakReferenceMessenger.Default.Send(new NavigationFinishedMessage(PageViewModel));
        }

        private async Task Navigate(BasePageViewModel basePageViewModel, TransitionModel? transition = null)
        {
            var transitionInfo = ConversionHelpers.ToNavigationTransitionInfo(transition) ?? new EntranceNavigationTransitionInfo();

            if (transition?.IsCustom ?? false)
            {
                if (transition is DrillOutTransitionModel)
                {
                    DrillOutAnimationStoryboard.Begin();
                    await Task.Delay(200);
                    DrillOutAnimationStoryboard.Stop();
                    transitionInfo = new EntranceNavigationTransitionInfo();
                }
            }

            switch (basePageViewModel)
            {
                case VaultLoginPageViewModel:
                    ContentFrame.Navigate(typeof(VaultLoginPage), basePageViewModel, transitionInfo);
                    break;

                case VaultDashboardPageViewModel:
                    ContentFrame.Navigate(typeof(VaultDashboardPage), basePageViewModel, transitionInfo);
                    break;

                default:
                    ContentFrame.Navigate(null, null);
                    break;
            }
        }

        public BasePageViewModel PageViewModel
        {
            get => (BasePageViewModel)GetValue(PageViewModelProperty);
            set => SetValue(PageViewModelProperty, value);
        }
        public static readonly DependencyProperty PageViewModelProperty =
            DependencyProperty.Register("PageViewModel", typeof(BasePageViewModel), typeof(NavigationControl), new PropertyMetadata(null));
    }
}
