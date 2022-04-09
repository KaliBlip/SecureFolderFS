﻿using CommunityToolkit.Mvvm.DependencyInjection;
using SecureFolderFS.Backend.Services;

namespace SecureFolderFS.Backend.Extensions
{
    public static class LocalizationExtensions
    {
        private static ILocalizationService? FallbackLocalizationService;

        public static string? ToLocalized(this string resourceKey, ILocalizationService? localizationService = null)
        {
            if (localizationService == null)
            {
                FallbackLocalizationService ??= Ioc.Default.GetService<ILocalizationService>();
                return FallbackLocalizationService?.LocalizeFromResourceKey(resourceKey) ?? string.Empty;
            }

            return localizationService.LocalizeFromResourceKey(resourceKey);
        }
    }
}