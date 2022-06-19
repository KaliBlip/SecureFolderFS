﻿using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SecureFolderFS.Sdk.Services;
using SecureFolderFS.WinUI.ServiceImplementation;
using SecureFolderFS.WinUI.WindowViews;
using SecureFolderFS.WinUI.Helpers;
using System.Threading.Tasks;
using Windows.Storage;
using SecureFolderFS.Sdk.Models;
using SecureFolderFS.Sdk.Services.Settings;
using SecureFolderFS.Sdk.Storage;
using SecureFolderFS.WinUI.ServiceImplementation.Settings;
using SecureFolderFS.WinUI.Serialization;
using SecureFolderFS.WinUI.Storage.NativeStorage;
using SecureFolderFS.WinUI.Storage.WindowsStorage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SecureFolderFS.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;

        private IServiceProvider? ServiceProvider { get; set; }

        /// <summary>
        /// Initializes the singleton application object. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            EnsureEarlyApp();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user. Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Get settings folder
            // TODO: Not ideal, use abstractions instead of getting hard implementation
            var settingsFolder = await GetSettingsFolderAsync();

            // Configure IoC
            ServiceProvider = ConfigureServices(settingsFolder);
            Ioc.Default.ConfigureServices(ServiceProvider);

            _window = new MainWindow();
            _window.Activate();
        }

        private void EnsureEarlyApp()
        {
            // Configure exception handlers
            UnhandledException += App_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            // Start AppCenter
            // TODO: Start AppCenter
        }

        private IServiceProvider ConfigureServices(IFolder settingsFolder)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddSingleton<ISettingsService, SettingsService>(_ => new SettingsService(settingsFolder.GetFilePool()!))
                .AddSingleton<IApplicationSettingsService, ApplicationSettingsService>(_ => new ApplicationSettingsService(settingsFolder.GetFilePool()!))
                .AddSingleton<IGeneralSettingsService, GeneralSettingsService>(sp => GetSettingsService(sp, (database, model) => new GeneralSettingsService(database, model)))
                .AddSingleton<IPreferencesSettingsService, PreferencesSettingsService>(sp => GetSettingsService(sp, (database, model) => new PreferencesSettingsService(database, model)))
                .AddSingleton<ISecuritySettingsService, SecuritySettingsService>(sp => GetSettingsService(sp, (database, model) => new SecuritySettingsService(database, model)))

                .AddSingleton<ISecretSettingsService, SecretSettingsService>(_ => new SecretSettingsService(settingsFolder.GetFilePool()!))
                .AddSingleton<IFileSystemService, NativeFileSystemService>()
                .AddSingleton<IDialogService, DialogService>()
                .AddSingleton<IApplicationService, ApplicationService>()
                .AddSingleton<IThreadingService, ThreadingService>()
                .AddSingleton<ILocalizationService, LocalizationService>()
                .AddSingleton<IFileExplorerService, FileExplorerService>()
                .AddSingleton<IClipboardService, ClipboardService>()
                .AddSingleton<IUpdateService, MicrosoftStoreUpdateService>();

            return serviceCollection.BuildServiceProvider(); // TODO: true?
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            LogException(e.Exception);
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            LogException(e.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            LogException(e.ExceptionObject as Exception);
        }

        private void LogException(Exception? ex)
        {
            var formattedException = ExceptionHelpers.FormatException(ex);

            Debug.WriteLine(formattedException);
            Debugger.Break(); // Please check "Output Window" for exception details (View -> Output Window) (Ctr + Alt + O)

#if !DEBUG
            ExceptionHelpers.LogExceptionToFile(formattedException);
#endif
        }

        private static TSettingsService GetSettingsService<TSettingsService>(IServiceProvider serviceProvider,
            Func<ISettingsDatabaseModel, ISettingsModel, TSettingsService> initializer)
            where TSettingsService : SharedSettingsModel
        {
            var settingsServiceImpl = serviceProvider.GetRequiredService<ISettingsService>() as SettingsService;

            return initializer(settingsServiceImpl!.GetDatabaseModel(), settingsServiceImpl!);
        }

        private static async Task<IFolder> GetSettingsFolderAsync()
        {
            const bool USE_NATIVE_STORAGE = true;

            if (USE_NATIVE_STORAGE)
            {
                return new NativeFolder(Path.Combine(ApplicationData.Current.LocalFolder.Path, Constants.LocalSettings.SETTINGS_FOLDER_NAME));
            }
            else
            {
                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(Constants.LocalSettings.SETTINGS_FOLDER_NAME, CreationCollisionOption.OpenIfExists);
                return new WindowsStorageFolder(folder);
            }
        }
    }
}
