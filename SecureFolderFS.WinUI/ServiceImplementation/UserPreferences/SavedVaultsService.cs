﻿using System.Collections.Generic;
using SecureFolderFS.Sdk.AppModels;
using SecureFolderFS.Sdk.Services.UserPreferences;
using SecureFolderFS.Sdk.Storage.StoragePool;
using SecureFolderFS.WinUI.AppModels;

namespace SecureFolderFS.WinUI.ServiceImplementation.UserPreferences
{
    /// <inheritdoc cref="ISavedVaultsService"/>
    internal sealed class SavedVaultsService : SingleFileSettingsModel, ISavedVaultsService
    {
        public SavedVaultsService(IFilePool? settingsFilePool)
        {
            FilePool = settingsFilePool;
            SettingsDatabase = new DictionarySettingsDatabaseModel(new SavedVaultsJsonToStreamSerializer());
            IsAvailable = settingsFilePool is not null;
        }

        /// <inheritdoc/>
        protected override string? SettingsStorageName { get; } = Constants.LocalSettings.SAVED_VAULTS_FILENAME;

        /// <inheritdoc/>
        public List<string>? VaultPaths
        {
            get => GetSetting<List<string>?>(null);
            set => SetSetting(value);
        }
    }
}
