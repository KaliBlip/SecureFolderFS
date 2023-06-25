﻿using CommunityToolkit.Mvvm.DependencyInjection;
using Octokit;
using SecureFolderFS.Sdk.AppModels;
using SecureFolderFS.Sdk.Services;
using SecureFolderFS.Sdk.ViewModels;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SecureFolderFS.UI.ServiceImplementation
{
    /// <inheritdoc cref="IChangelogService"/>
    public sealed class GitHubChangelogService : IChangelogService
    {
        private IApplicationService ApplicationService { get; } = Ioc.Default.GetRequiredService<IApplicationService>();

        /// <inheritdoc/>
        public async Task<ChangelogViewModel?> GetChangelogAsync(AppVersion version, CancellationToken cancellationToken)
        {
            const string repoName = Constants.GitHub.REPOSITORY_NAME;
            const string repoOwner = Constants.GitHub.REPOSITORY_OWNER;

            try
            {
                var client = new GitHubClient(new ProductHeaderValue(repoOwner));
                var release = await client.Repository.Release.Get(repoOwner, repoName, version.Version.ToString());

                return new(release.Name, FormatBody(release.Body), version.Version);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ChangelogViewModel> GetChangelogSinceAsync(AppVersion version, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            const string repoName = Constants.GitHub.REPOSITORY_NAME;
            const string repoOwner = Constants.GitHub.REPOSITORY_OWNER;

            IReadOnlyList<Release>? releases;
            var currentVersion = ApplicationService.GetAppVersion();

            try
            {
                var client = new GitHubClient(new ProductHeaderValue(repoOwner));
                releases = await client.Repository.Release.GetAll(repoOwner, repoName);
            }
            catch (Exception)
            {
                yield break;
            }

            foreach (var item in releases)
            {
                if (item.Draft)
                    continue;

                var itemVersion =
                    new Version(item.TagName.Replace("v", string.Empty, StringComparison.OrdinalIgnoreCase));

                // 'itemVersion' must be same or newer than 'version' as well as same or older than 'currentVersion'
                if (itemVersion >= version.Version && itemVersion <= currentVersion.Version)
                    yield return new(item.Name, FormatBody(item.Body), itemVersion);
            }
        }

        private static string FormatBody(string changelogBody)
        {
            return changelogBody.Replace("\r\n", "\r\n\n");
        }
    }
}