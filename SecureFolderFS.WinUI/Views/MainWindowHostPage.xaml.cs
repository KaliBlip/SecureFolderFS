﻿using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SecureFolderFS.Sdk.Messages;
using SecureFolderFS.Sdk.Models.Transitions;
using SecureFolderFS.Sdk.ViewModels;
using SecureFolderFS.Sdk.ViewModels.Sidebar;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SecureFolderFS.WinUI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    internal sealed partial class MainWindowHostPage : Page, IRecipient<RemoveVaultRequestedMessage>
    {
        public MainViewModel ViewModel
        {
            get => (MainViewModel)DataContext;
            set => DataContext = value;
        }

        public MainWindowHostPage()
        {
            InitializeComponent();

            ViewModel = new();

            WeakReferenceMessenger.Default.Register<RemoveVaultRequestedMessage>(this);
        }

        public void Receive(RemoveVaultRequestedMessage message)
        {
            ViewModel.SidebarViewModel.SelectedItem = ViewModel.SidebarViewModel.SidebarItems.FirstOrDefault();
        }

        private void Sidebar_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is SidebarItemViewModel itemViewModel)
            {
                WeakReferenceMessenger.Default.Send(new NavigationRequestedMessage(itemViewModel.VaultViewModel) { Transition = new EntranceTransitionModel() });
            }
        }

        private void MainWindowHostPage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.EnsureLateApplication();
        }
    }
}
