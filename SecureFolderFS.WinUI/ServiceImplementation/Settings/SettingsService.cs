﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage;
using SecureFolderFS.Sdk.Models;
using SecureFolderFS.Sdk.ViewModels;
using SecureFolderFS.WinUI.Serialization;
using SecureFolderFS.WinUI.Serialization.Implementation;
using SecureFolderFS.Shared.Extensions;
using SecureFolderFS.Sdk.Services.Settings;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace SecureFolderFS.WinUI.ServiceImplementation.Settings
{
    internal sealed class SettingsService : BaseJsonSettings, ISettingsService
    {
        private IGeneralSettingsService? _GeneralSettingsService;
        public IGeneralSettingsService GeneralSettingsService
        {
            get => GetSettingsService(ref _GeneralSettingsService);
        }

        private IPreferencesSettingsService? _PreferencesSettingsService;
        public IPreferencesSettingsService PreferencesSettingsService
        {
            get => GetSettingsService(ref _PreferencesSettingsService);
        }

        private ISecuritySettingsService? _SecuritySettingsService;
        public ISecuritySettingsService SecuritySettingsService
        {
            get => GetSettingsService(ref _SecuritySettingsService);
        }

        public Dictionary<VaultIdModel, VaultViewModel> SavedVaults
        {
            get => Get<List<KeyValuePair<VaultIdModel, VaultViewModel>>>(() => new())!.ToDictionary()!;
            set => Set<List<KeyValuePair<VaultIdModel, VaultViewModel>>>(value.ToList());
        }

        public SettingsService()
        {
            SettingsSerializer = new DefaultSettingsSerializer();
            JsonSettingsSerializer = new DefaultJsonSettingsSerializer();
            JsonSettingsDatabase = new CachingJsonSettingsDatabase(SettingsSerializer, JsonSettingsSerializer);

            Initialize(Path.Combine(ApplicationData.Current.LocalFolder.Path, Constants.LocalSettings.SETTINGS_FOLDER_NAME, Constants.LocalSettings.USER_SETTINGS_FILE_NAME));
        }

        private TSettingsService GetSettingsService<TSettingsService>(ref TSettingsService? settingsServiceMember)
            where TSettingsService : class, IBaseSettingsService
        {
            settingsServiceMember ??= Ioc.Default.GetRequiredService<TSettingsService>();
            return settingsServiceMember;
        }

        public TSharingContext GetSharingContext<TSharingContext>()
            where TSharingContext : class
        {
            return (TSharingContext)base.GetSharingContext();
        }
    }
}
