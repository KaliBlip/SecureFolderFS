﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SecureFolderFS.Sdk.ViewModels.Pages.Settings;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SecureFolderFS.WinUI.Views.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PreferencesSettingsPage : Page
    {
        public PreferencesSettingsPageViewModel ViewModel
        {
            get => (PreferencesSettingsPageViewModel)DataContext;
            set => DataContext = value;
        }

        public PreferencesSettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is PreferencesSettingsPageViewModel viewModel)
                ViewModel = viewModel;

            base.OnNavigatedTo(e);
        }

        private void PreferencesSettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.BannerViewModel.UpdateAdapterStatus();
        }

        private void FileSystemComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.BannerViewModel.UpdateAdapterStatus();
        }
    }
}
