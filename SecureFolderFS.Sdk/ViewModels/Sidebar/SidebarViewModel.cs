﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SecureFolderFS.Sdk.Messages;
using SecureFolderFS.Sdk.Models;
using SecureFolderFS.Sdk.Models.Transitions;
using SecureFolderFS.Sdk.Services;
using SecureFolderFS.Sdk.Utils;
using SecureFolderFS.Sdk.ViewModels.Dialogs;

namespace SecureFolderFS.Sdk.ViewModels.Sidebar
{
    public sealed class SidebarViewModel : ObservableObject, IInitializableSource<IDictionary<VaultIdModel, VaultViewModel>>, IRecipient<RemoveVaultRequestedMessage>, IRecipient<AddVaultRequestedMessage>
    {
        private readonly SearchModel<SidebarItemViewModel> _sidebarSearchModel;

        private IDialogService DialogService { get; } = Ioc.Default.GetRequiredService<IDialogService>();

        private IThreadingService ThreadingService { get; } = Ioc.Default.GetRequiredService<IThreadingService>();

        public ObservableCollection<SidebarItemViewModel> SidebarItems { get; }

        private SidebarItemViewModel? _SelectedItem;
        public SidebarItemViewModel? SelectedItem
        {
            get => _SelectedItem;
            set => SetProperty(ref _SelectedItem, value);
        }

        private string? _SearchQuery;
        public string? SearchQuery
        {
            get => _SearchQuery;
            set
            {
                if (SetProperty(ref _SearchQuery, value))
                {
                    SearchQueryChanged(value);
                }
            }
        }

        private bool _NoItemsFoundLoad;
        public bool NoItemsFoundLoad
        {
            get => _NoItemsFoundLoad;
            set => SetProperty(ref _NoItemsFoundLoad, value);
        }

        public IAsyncRelayCommand CreateNewVaultCommand { get; }

        public IAsyncRelayCommand OpenSettingsCommand { get; }

        public SidebarViewModel()
        {
            SidebarItems = new();
            CreateNewVaultCommand = new AsyncRelayCommand(CreateNewVault);
            OpenSettingsCommand = new AsyncRelayCommand(OpenSettings);
            _sidebarSearchModel = new()
            {
                Collection = SidebarItems,
                FinderPredicate = (item, key) => item.VaultName!.ToLowerInvariant().Contains(key)
            };

            WeakReferenceMessenger.Default.Register<RemoveVaultRequestedMessage>(this);
            WeakReferenceMessenger.Default.Register<AddVaultRequestedMessage>(this);
        }

        public void Receive(RemoveVaultRequestedMessage message)
        {
            var itemToRemove = SidebarItems.FirstOrDefault(item => item.VaultViewModel.VaultIdModel == message.Value);
            if (itemToRemove is not null)
            {
                SidebarItems.Remove(itemToRemove);
            }
        }

        public void Receive(AddVaultRequestedMessage message)
        {
            SidebarItems.Add(new(message.Value));
        }

        void IInitializableSource<IDictionary<VaultIdModel, VaultViewModel>>.Initialize(IDictionary<VaultIdModel, VaultViewModel> param)
        {
            _ = ThreadingService.ExecuteOnUiThreadAsync(() =>
            {
                foreach (var item in param.Values)
                {
                    SidebarItems.Add(new(item));
                }

                if (SidebarItems.FirstOrDefault() is SidebarItemViewModel itemToSelect)
                {
                    SelectedItem = itemToSelect;
                    WeakReferenceMessenger.Default.Send(new NavigationRequestedMessage(itemToSelect.VaultViewModel) { Transition = new SuppressTransitionModel() });
                }
            });
        }

        private async Task CreateNewVault()
        {
            SearchQuery = string.Empty;

            var vaultWizardViewModel = new VaultWizardDialogViewModel();
            await DialogService.ShowDialog(vaultWizardViewModel);
        }

        private async Task OpenSettings()
        {
            await DialogService.ShowDialog(new SettingsDialogViewModel());
        }

        private void SearchQueryChanged(string? query)
        {
            NoItemsFoundLoad = !_sidebarSearchModel.SubmitQuery(query);
        }
    }
}
