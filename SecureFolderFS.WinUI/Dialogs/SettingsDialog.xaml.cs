﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SecureFolderFS.Sdk.Enums;
using SecureFolderFS.Sdk.Extensions;
using SecureFolderFS.Sdk.Models;
using SecureFolderFS.Sdk.ViewModels.Dialogs;
using SecureFolderFS.Sdk.ViewModels.Views;
using SecureFolderFS.Sdk.ViewModels.Views.Settings;
using SecureFolderFS.WinUI.ServiceImplementation;
using System;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SecureFolderFS.WinUI.Dialogs
{
    public sealed partial class SettingsDialog : ContentDialog, IDialog<SettingsDialogViewModel>
    {
        /// <inheritdoc/>
        public SettingsDialogViewModel ViewModel
        {
            get => (SettingsDialogViewModel)DataContext;
            set => DataContext = value;
        }

        public SettingsDialog()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public new async Task<DialogResult> ShowAsync() => (DialogResult)await base.ShowAsync();

        private INavigationTarget GetTargetForTag(int tag)
        {
            return tag switch
            {
                0 => ViewModel.NavigationService.TryGetTarget<GeneralSettingsViewModel>() ?? new(),
                1 => ViewModel.NavigationService.TryGetTarget<PreferencesSettingsViewModel>() ?? new(),
                2 => ViewModel.NavigationService.TryGetTarget<PrivacySettingsViewModel>() ?? new(),
                3 => ViewModel.NavigationService.TryGetTarget<AboutSettingsViewModel>() ?? new(),
                _ => new GeneralSettingsViewModel()
            };
        }

        private bool SetupNavigation()
        {
            if (ViewModel.NavigationService.IsInitialized)
                return true;

            if (ViewModel.NavigationService is WindowsNavigationService navigationServiceImpl)
            {
                navigationServiceImpl.NavigationControl = Navigation;
                return navigationServiceImpl.NavigationControl is not null;
            }

            return false;
        }

        private async void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (!SetupNavigation())
                return;

            var tag = Convert.ToInt32((args.SelectedItem as NavigationViewItem)?.Tag);
            var target = GetTargetForTag(tag);

            await ViewModel.NavigationService.NavigateAsync(target);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void SettingsDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            // Remove the reference to the NavigationControl so the dialog can get properly garbage collected
            if (ViewModel.NavigationService is WindowsNavigationService navigationServiceImpl)
                navigationServiceImpl.NavigationControl = null;
        }
    }
}
